ASP.NET Web API CacheOutput - MongoDb
================================

MongoDb 'provider' for AspNetWebApi-OutputCache  
Implementation of `IApiOutputCache` for MongoDb

Usage
--------------------

Simply create an instance of `MongoDbApiOutputCache`

`MongoDbApiOutputCache` takes a `MongoDatabase` in the ctor, so you can pass this in via IoC.  
You can also specify your cache collection name. Default is "cache".

From the [AspNetWebApi-OutputCache readme](https://github.com/filipw/AspNetWebApi-OutputCache/wiki):
>
You can register your implementation using a handy *GlobalConfiguration* extension method:

    //instance
    configuration.CacheOutputConfiguration().RegisterCacheOutputProvider(() => new MyCache());

    //singleton
    var cache = new MyCache();
    configuration.CacheOutputConfiguration().RegisterCacheOutputProvider(() => cache);	

>If you prefer **CacheOutput** to use resolve the cache implementation directly from your dependency injection provider, that's also possible. Simply register your *IApiOutputCache* implementation in your Web API DI and that's it. Whenever **CacheOutput** does not find an implementation in the *GlobalConiguration*, it will fall back to the DI resolver. Example (using Autofac for Web API):

    cache = new MyCache();
    var builder = new ContainerBuilder();
    builder.RegisterInstance(cache);
    config.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());
