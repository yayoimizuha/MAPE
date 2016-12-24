﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MAPE.ComponentBase;
using MAPE.Http;
using MAPE.Utils;

namespace MAPE.Server {
    public class Connection: TaskingComponent, ICommunicationOwner {
		#region constants

		public const string ObjectBaseName = "Connection";

		#endregion


		#region data

		private ConnectionCollection owner = null;

		#endregion


		#region data synchronized by classLocker

		private static object classLocker = new object();

		private static int nextId = 0;

		#endregion


		#region data - synchronized by locking this

		private int id = 0;

		private int retryCount;

		private TcpClient client = null;

		private TcpClient server = null;

		private byte[] proxyCredential = null;

		#endregion


		#region properties

		public Proxy Proxy {
			get {
				return this.owner.Owner;
			}
		}

		public ComponentFactory ComponentFactory {
			get {
				return this.owner.ComponentFactory;
			}
		}

		#endregion


		#region creation and disposal

		public Connection() {
			// initialize members
			this.ObjectName = ObjectBaseName;

			return;
		}

		public override void Dispose() {
			// stop communicating
			StopCommunication();
		}


		public void ActivateInstance(ConnectionCollection owner) {
			// argument checks
			Debug.Assert(owner != null);

			lock (this) {
				// state checks
				if (this.owner != null) {
					throw new InvalidOperationException("The instance is in use.");
				}

				// initialize members
				this.owner = owner;
				this.retryCount = owner.Owner.RetryCount;
				Debug.Assert(this.client == null);
				Debug.Assert(this.server == null);
				Debug.Assert(this.proxyCredential == null);
				lock (classLocker) {
					this.id = nextId++;
				}
			}

			return;
		}

		public void DeactivateInstance() {
			lock (this) {
				// state checks
				if (this.owner == null) {
					// already deactivated
					return;
				}
				if (this.client != null) {
					throw new InvalidOperationException("The instance is still working.");
				}

				// uninitialize members
				Debug.Assert(this.proxyCredential == null);
				Debug.Assert(this.server == null);
				Debug.Assert(this.client == null);
				this.owner = null;
				this.Task = null;
			}

			return;
		}

		#endregion


		#region methods

		public void StartCommunication(TcpClient client) {
			// argument checks
			if (client == null) {
				throw new ArgumentNullException(nameof(client));
			}

			try {
				lock (this) {
					// log
					LogInformation($"Starting for {client.Client.RemoteEndPoint.ToString()} ...");

					// state checks
					if (this.owner == null) {
						throw new ObjectDisposedException(this.ObjectName);
					}

					Task communicatingTask = this.Task;
					if (communicatingTask != null) {
						throw new InvalidOperationException("It already started communication.");
					}
					communicatingTask = new Task(Communicate);
					communicatingTask.ContinueWith(
						(Action<Task>)((t) => {
							base.LogInformation((string)"Stopped.");
							this.ObjectName = ObjectBaseName;
							this.owner.OnConnectionCompleted(this);
						})
					);
					this.Task = communicatingTask;

					this.ObjectName = $"{ObjectBaseName} <{this.id}>";
					Debug.Assert(this.client == null);
					this.client = client;

					// start communicating task
					communicatingTask.Start();

					// log
					LogInformation("Started.");
				}
			} catch (Exception exception) {
				LogError($"Fail to start: {exception.Message}");
				throw;
			}

			return;
		}

		public bool StopCommunication(int millisecondsTimeout = 0) {
			bool stopConfirmed = false;
			try {
				Task communicatingTask;
				lock (this) {
					// state checks
					if (this.owner == null) {
						throw new ObjectDisposedException(this.ObjectName);
					}

					communicatingTask = this.Task;
					if (communicatingTask == null) {
						// already stopped
						return true;
					}
					LogInformation("Stopping...");

					// force the connections to close
					// It will cause exceptions on I/O in communicating thread.
					if (this.server != null) {
						Socket socket = this.server.Client;
						socket.Shutdown(SocketShutdown.Both);
						socket.Disconnect(false);
					}
					if (this.client != null) {
						Socket socket = this.client.Client;
						socket.Shutdown(SocketShutdown.Both);
						socket.Disconnect(false);
					}
				}

				// wait for the completion of the listening task
				// Note that -1 timeout means 'Infinite'.
				if (millisecondsTimeout != 0) {
					stopConfirmed = communicatingTask.Wait(millisecondsTimeout);
				}

				// log
				// "Stopped." will be logged in the continuation of the communicating task. See StartCommunication().
			} catch (Exception exception) {
				LogError($"Fail to stop: {exception.Message}");
				throw;
			}

			return stopConfirmed;
		}

		#endregion


		#region overridables

		protected virtual IEnumerable<MessageBuffer.Modification> GetModifications(Request request, Response response) {
			// argument checks
			if (request == null) {
				throw new ArgumentNullException(nameof(request));
			}
			// response may be null

			// ToDo: thread protection
			byte[] overridingProxyCredential;
			if (response == null) {
				// first request
				if (request.ProxyAuthorizationSpan.IsZeroToZero == false) {
					// the client specified Proxy-Authorization
					overridingProxyCredential = null;
				} else {
					overridingProxyCredential = this.proxyCredential;
					if (overridingProxyCredential == null) {
						overridingProxyCredential = this.Proxy.GetProxyCredential(null, false);
					}
				}
			} else {
				// re-sending request
				if (response.StatusCode == 407) {
					// the current credential seems to be invalid
					overridingProxyCredential = this.Proxy.GetProxyCredential(response.ProxyAuthenticateValue, true);
				} else {
					// no need to resending
					overridingProxyCredential = null;
				}
			}

			MessageBuffer.Modification[] modifications;
			if (overridingProxyCredential == null) {
				modifications = null;
			} else {
				modifications = new MessageBuffer.Modification[] {
					new MessageBuffer.Modification(
						request.ProxyAuthorizationSpan.IsZeroToZero? request.EndOfHeaderFields: request.ProxyAuthorizationSpan,
						(mb) => { mb.Write(overridingProxyCredential); return true; }	
					)
				};
			}

			return modifications;
		}

		#endregion


		#region ICommunicationOwner - for Communication class only

		ComponentFactory ICommunicationOwner.ComponentFactory {
			get {
				return this.ComponentFactory;
			}
		}

		ILogger ICommunicationOwner.Logger {
			get {
				return this;
			}
		}

		IEnumerable<MessageBuffer.Modification> ICommunicationOwner.GetModifications(int repeatCount, Request request, Response response) {
			// argument checks
			if (request == null) {
				throw new ArgumentNullException(nameof(request));
			}
			if (response == null && repeatCount != 0) {
				throw new ArgumentNullException(nameof(response));
			}

			// log
			if (response == null) {
				LogInformation($"Request {request.Method} to '{request.Host}'");
			} else {
				int statusCode = response.StatusCode;
				string message = $"Response {statusCode.ToString()}";
				if (statusCode < 300) {
					LogInformation(message);
				} else if (statusCode < 400 || statusCode == 407) {
					LogWarning(message);
				} else {
					LogError(message);
				}
			}

			// retry checks
			IEnumerable<MessageBuffer.Modification> modifications = null;
			if (repeatCount <= this.retryCount) {
				// get actual modifications
				modifications = GetModifications(request, response);
			}

			// log
			if (modifications == null) {
				// the response will be sent to the client
				LogInformation("Responded.");
			}

			return modifications;
		}

		void ICommunicationOwner.OnClose(bool downstream, Exception error) {
			if (error != null) {
				StopCommunication();
			} else {
				lock (this) {
					if (this.server != null) {
						Socket socket = this.server.Client;
						socket.Shutdown(downstream? SocketShutdown.Receive: SocketShutdown.Send);
					}
					if (this.client != null) {
						Socket socket = this.client.Client;
						socket.Shutdown(downstream ? SocketShutdown.Send : SocketShutdown.Receive);
					}
				}
			}

			return;
		}

		#endregion


		#region privates

		private void Communicate() {
			// preparations
			ConnectionCollection owner;
			Proxy proxy;
			TcpClient client;
			TcpClient server;
			Exception openServerError;
			lock (this) {
				owner = this.owner;
				proxy = this.Proxy;
				client = this.client;
				try {
					server = proxy.OpenServerConnection(client);
					openServerError = null;
				} catch (Exception exception) {
					LogWarning($"Fail to connect the server: {exception.Message}");
					LogWarning($"Sending an error response to the client.");
					server = null;
					openServerError = exception;
					// continue
				}
				this.server = server;
			}

			// communicate
			try {
				using (NetworkStream clientStream = this.client.GetStream()) {
					if (openServerError != null) {
						// the case that server connection is not available
						Response.RespondSimpleError(clientStream, 500, "Not Connected to Actual Proxy");
						LogError($"Cannot connect to the actual proxy '{this.Proxy.Server.Host}:{this.Proxy.Server.Port}'.");
					} else {
						using (NetworkStream serverStream = server.GetStream()) {
							Communication.Communicate(this, clientStream, serverStream);
						}
					}
				}
				LogInformation("Communication completed.");
			} catch (EndOfStreamException) {
				// the end of communication
				// continue
			} catch (Exception exception) {
				// A 400 (Bad Request) error has been sent to client at this point. 
				LogError($"Fail to communicate: {exception.Message}");
				// continue
			} finally {
				lock (this) {
					this.proxyCredential = null;
					this.server = null;
					this.client = null;
				}
				if (server != null) {
					try {
						server.Close();
					} catch {
						// continue
					}
				}
				try {
					client.Close();
				} catch {
					// continue
				}
			}

			return;
		}

		#endregion
	}
}
