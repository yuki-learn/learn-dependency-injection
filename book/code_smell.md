# コードの嫌な匂い

## コンストラクタ経由での過度な注入
* DIはコンストラクタ注入が基本となるが、クラスでのコンストラクタの注入の数が多くなってしまうこと。
大体は、注入の数が多いのは単一責任の原則が守られていないからそうなっているのでそれを解消する。
* コンストラクタ注入にすることによって単一責任の原則が守られていないことが発見できることも。
* コンストラクタの引数が4つを超えたら設計を疑うべき。

## 抽象ファクトリ(abstract factory)の誤用


### 1. オブジェクトのlifetimeの問題を解決しようとすることで生じる抽象ファクトリの誤用


よくある、引数なしの生成メソッドを持つ抽象ファクトリ

```cs
public class IProductRepositoryFactory
{
    // 揮発性依存のオブジェクトを生成して返す、引数なしの生成メソッド
    IProductRepository Create();
}
```

#### 抽象ファクトリを利用する悪いコード
```cs
public class HomeController : Controller
{
    private readonly IProductRepositoryFactory factory;

    public HomeController(IProductRepositoryFactory factory)
    {
        this.factory = factory;
    }

    public ViewResult Index()
    {
        // 抽象ファクトリでリポジトリを生成して、リポジトリの生存管理を行う。
        using (IProductRepository repository = this.factory.Create())
        {
            // このリポジトリはIDisposableを継承してるから使用後は破棄する必要あり。
            var products = repository.GetProducts();
            return this.View(products);
        }
    }
}
```

一見、普通の実装に見えてしまうが、コードを利用する側は`IProductRepository`を使ったら破棄しないといけないことを知らなければならず、リポジトリとして抽象化してるのにそれが完全ではない。 <br>
このように、抽象化してるのに詳細を知らないといけない抽象を**漏洩する抽象**という。


**抽象を抽出する際は、想定している状況とは異なる状況であっても、その抽象が意味を成すものになるのか、ということを常に考えておく。**


抽象に対して、`IDisposable`インターフェイスを持たせるべきではない。
なぜなら、その抽象を利用するコードは抽象化された内容しかしらないわけで、オブジェクトの生存管理の責務を担うことはできないから。(クラスに対して`IDisposable`インターフェイスを実装するなということではない。)

抽象ファクトリはこういった漏洩する抽象を作りやすい。



#### リファクタリングした良いコード

```cs
public class HomeController : Controller
{
    private readonly IProductRepository factory;

    // 抽象ファクトリを注入するのではなく、リポジトリを直接注入してしまう。
    public HomeController(IProductRepository repository)
    {
        this.repository = repository;
    }

    public ViewResult Index()
    {
        // 利用する側で生存管理を行わなず、別の場所でやることにする
        var products = repository.GetProducts();
        return this.View(products);
    }
}
```

#### Proxyパターンを使って別クラスに生存管理させる
```cs
public class SqlProductRepositoryProxy : IProductRepository
{
    private readonly string connectionString;

    public SqlProductRepositoryProxy(string connectionString)
    {
        this.connectionString = connectionString;
    }

    // Proxyパターンでオブジェクトの生成を遅延させる。
    public IEnumrable<Product> GetProducts()
    {
        using (var repository = this.Create())
        {
            // 実際リポジトリの呼び出し
            return repository.GetProducts();
        }
    }

    private SqlProductRepository Create()
    {
        // Proxyクラスが生成するように。
        return new SqlProductRepository(this.connectionString);
    }
}
```




## 循環依存の取り除き方

