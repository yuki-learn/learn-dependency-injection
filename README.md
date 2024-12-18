
# なぜ依存を注入するのか メモ

## 依存注入とは

依存注入とは目的を達成するための手段である。
どんな目的か？**保守容易性を高めること**

保守容易性を高める方法の1つに、**疎結合**な設計にするというのがあり、依存注入をすることで疎結合にできる。


### 依存注入のよくある誤解
よくある誤解4つ。

* 依存注入は遅延バインディング(late binding)を行うときにしか用いられない。
    * ここで言う遅延バインディングとは、コードを再コンパイルすることなくアプリの一部の実装を変更できる能力のこと。
    * 遅延バインディングを実現するために依存注入をするというのは誤解。遅延バインディングは依存注入の可能性の1つであって、これだけが目的ではない。
* 依存注入は単体テストをするときにだけ関係してくる。
    * 違う。依存注入によって単体テストがやりやすくなるのは事実だけど、単体テストのために導入するものではない。
* 依存注入はドーピングをしたAbstract Factoryパターンのようなものである
* 依存注入を導入するにはDIコンテナが必要である。
    * 必須ではない。依存注入はDIコンテナを使うことで楽になるが、なくても可能である。


### 疎結合にするメリット

| メリット                         | 概要               | どんなとき価値が出るか。 | 
| -------------------------------- | ------------------ | ------------------------ | 
| 遅延バインディング(late binding) | 使用するサービスをコンパイル時ではなく、実行時に決められるようになる。実装の差し替え時、再コンパイルする必要がない。 |            様々な場面で使われるソフトウェアだとメリットがあるが、実行環境が前もって決められているエンタープライズアプリケーションだとあまり効果がないかも。              | 
|               拡張容易性(extensibility)                   |既存のコードを拡張したたり、再利用しやすくなる。|常に価値がある。| 
|並列開発性(parallel development)|異なる機能の開発を並列して行える。|大規模なほど効果がある。小規模だとあまり効果ないかも| 
|保守容易性(maintanability)|クラスの責務が明確に定義されるため、保守が行いやすくなる。|常に価値がある。| 
|テスト容易性(testability)|単体テストがしやすくなる。|常に価値がある。| 


