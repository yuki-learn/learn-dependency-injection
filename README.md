# なぜ依存を注入するのか DIの原理・原則とパターンを読んで


## 本のメモ
本を読んだメモ: 
https://github.com/yuki-learn/learn-dependency-injection/tree/main/book

## サンプルアプリ
本に書かれていたサンプルコードを使用して、自分なりに修正し、DIの設計パターンとDIコンテナであるSimpleInjectorについて学んだ。

https://github.com/yuki-learn/learn-dependency-injection/tree/main/app



### 実行環境
.NET 8

※Windows環境で動かすには[SQLite](https://github.com/yuki-learn/learn-dependency-injection/blob/main/app/src/Commerce.Web.SimpleInjector/appsettings.json#L3)のパスを以下に修正する必要があります。
```json
"Data Source=..\\..\\sqlite.db"
```