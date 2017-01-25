[目次](Index.md)

---

# よくある質問

「よくある質問」といいつつ、実際は自分向けの覚え書きです。


### Q: これはFidllerみたいなものですか？

まあ、単機能のFidllerみたいなものかと言えば、そうかも。

認証プロキシを通すためにFiddlerを使ってみたりもしましたが、
以下の理由から別にツールを作りました。

* スクリプトをいじる必要（しかもちょっと複雑）があり、
非開発者な人に「こうやればなんとかできます」とか言いにくい。
* Fiddlerのプロキシ設定書き換えは、
「次で始まるアドレスにはプロキシを使用しない」を考慮してくれない。
まあ、ツールの目的からすれば、http通信をいったん全部引き受けるのは分かります。
が、認証プロキシが設置されているような環境では、
これがトラブルの原因になるケースがある。

それはそれとして、
Fiddlerはデバッグツールとして便利に利用させていただいております。


### Q: Fidller Core使っているの？

使っていません。

Fiddler Coreは個人利用だと無料だけど、
社内利用や商用利用は有償になります。
ライセンス条件見てしばらく考えましたが、
「認証プロキシをちゃんと通す」というシーンを考えると
業務利用扱いになっちゃいそうなので、
httpを処理する部分も作りました。


### Q: 初期設定（/Save /ProxyOverride）が面倒なんだけど、自動化できない？

ごもっともですが、ちょっと実装が面倒。

問題は自動構成スクリプト（.pac）が使われている場合です。
認証プロキシが導入されているような環境ではほとんど使われているんじゃないかな。
この場合、「次で始まるアドレスにはプロキシを使用しない」に相当する判定を行うには、
Javascriptエンジンをロードして.pacファイルを実行させてみる必要があります。

できなくはないはずだけど、
というか.NET Frameworkは内部的にやっているっぽいので、
その処理を再利用できるようにしてくれれば可能なんだけど、
そこ頑張るほどじゃないかなあ、と。


### Q: 名前はMAPLEとかにした方が響きがよくない？

まあそうかもしれませんが、MAPLEってソフトウェアは既にあるんですよ。