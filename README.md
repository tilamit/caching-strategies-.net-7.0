﻿﻿﻿<h2 align="center">
    <a style="text-decoration:none;" href="https://github.com/tilamit/caching-strategies-.net-7.0">
      Caching Strategies With .NET
    </a>
    <br/>
</h2>

## Project Goal

To demonstrate caching strategies with .NET

<p align="center">
    <img src="https://i.ibb.co/kDvj779/caching.png" alt="locking-strategies-.net-7.0" />
</p>

 <h4>Image Ref: 
  <a style="text-decoration:none;" href="https://gcore.com">
      GCORE
  </a>
 </h4>

## Scenario

Caching is one of the prominent elements of an app that improves performance over time. To know more about caching, we have to understand the fundamental working of it.

When we work on a project, say a database-driven one, in that case user sends request to the server and in response, server gets desired data to the user. Now think of a situation when multiple users requests for the same data over time and the app hits the database each time for this. This is pretty costly, in a sense if the data remains the same each time with no further changes. This is where **Caching** comes into play. It caches or stores data in a storage on server memory or client side say, browser. There are two main strategies that we can follow to implement caching:

* In-memory / Client Caching
* Distributed Caching

**In-memory / Client Caching** - These two can be implemented in the application-level, that means the cache could be controlled in the server-side and on client-side. For server-side, the app creates an object that occupies storage on the server itself and later that storage serves as the cache. On the other hand, for client-side, the cache is handled by browser and data gets retrieved from browser cache for later usage. 

**Distributed Caching** - For distributed caching, we can consider a separate server that deals with caching and even it can have multiple servers to distribute the caching mechanism for better app performance. In this scenario, caching isn't bound to application-level and each server works as a gateway for caching service.

## Built With

#### Environment & Development Tools：

* .NET Core Version: 7.0
* IDE: Visual Studio 2022
* Framework: .NET Core Web Api
* Backend: C# 11, ORM - Entity Framework Core Database First Approach 
* Database: MS SQL Server 2019
* Api Testing: JMeter

## Getting Started

Initially the project will restore all the required nuget packages for the project. If not, the following will help to make it done manually. 

For example, to install entity framework core and sql server package from nuget, the following command will download the specific version for the project.

### Prerequisites

* Nuget Package Manager

```sh
PM> Install-Package Microsoft.EntityFrameworkCore -Version 7.0.0
```

```sh
PM> Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 7.0.0
```

* SQL Server Database

For database, create the following table that I used for this specific sample and you can use yours as well depending upon use cases.

~~~ sql
USE [SampleDb]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 19/06/2024 16:51:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](40) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
~~~

To test the performance, I added dummy data in the table. You guys can use it.

~~~ sql
declare @id int 
select @id = 1
while @id >=1 and @id <= 1000
begin
    insert into Users values(@id, 'John ' + convert(nvarchar(5), @id))
    select @id = @id + 1
end;

select * from Users m;
~~~

The above will generate thousand dummy data in the **Users** table and it can be modified to generate more dummy data if required.

* Redis

For distributed caching, **Redis** has been used and to make it work in the project, an open source library **EasyCaching** is integrated. You can have a look on the library here - [EasyCaching](https://github.com/dotnetcore/EasyCaching).

To work with **Redis**, I used docker container. In order to run the container, you have to install docker / docker desktop. Then run the following in command prompt to pull the docker image as follows (I do it mostly with admin privilege):

```sh
docker pull redis
```

Once done, use the following to run the **Redis** container:

```sh
docker run --name redis-2 -d -p 6380:6379 redis
```

```sh
docker run --name redis-4 -d -p 6382:6379 redis
```

I created two instances of **Redis** with different ports to test the caching. By default, it uses the port 6379.

Now we are done with the setup and you can test with JMeter now.

* JMeter

To test the api and see the demo of caching strategy, you can use JMeter that I used. Here's a link that you can follow - [JMeter Setup](https://loadium.com/blog/how-to-send-jmeter-post-requests).

It's pretty handy and you can download it from here - [Download JMeter](https://jmeter.apache.org/download_jmeter.cgi). 

When installation done, do the followings to work with the project as shown in the image:

Threads (Users) -> Thread Group             |  HTTP Post Request
:-------------------------:|:-------------------------:
![](https://i.postimg.cc/NjHLGVTN/HTTP-Request-jmx-C-Users-Hp-Downloads-HTTP-Request-jmx-Apache-JMeter-5-6-3-19-06-2024-17-20-5.png)  |  ![](https://i.postimg.cc/NjmfvXqS/HTTP-Request-jmx-C-Users-Hp-Downloads-HTTP-Request-jmx-Apache-JMeter-5-6-3-19-06-2024-17-22-3.png)

* Caching With SQL Server

We can implement distributed caching with the help of SQL Server. In this case and for testing purpose, we would be using docker container on Linux environment or local SQL Server could be used as well.

This is what I used for the docker container as follows:

```sh
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=MySuperStrongPassword1!' -p 1432:1433 --name sqltest -d mcr.microsoft.com/mssql/server:2017-latest-ubuntu
```

The above helped me a lot when I was exploring for the SQL Server docker container and faced issues. It would be incomplete if I don't provide the credit and you guys can check it out if you face the same issue:

* [Chirag Darji](https://medium.com/@chiragrdarji/docker-container-sql-server-exited-1-status-b7d7131c289b)

Install the below package in the project:

```sh
PM> Install-Package Microsoft.Extensions.Caching.SqlServer
```

Use the following table for the caching strategy to store the cached data when you successfully start the docker container for SQL Server or the one you locally have:

~~~ sql
USE [SampleDb]
GO

/****** Object:  Table [dbo].[CacheData]    Script Date: 30/11/2024 18:15:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CacheData](
	[Id] [nvarchar](449) NOT NULL,
	[Value] [varbinary](max) NOT NULL,
	[ExpiresAtTime] [datetimeoffset](7) NOT NULL,
	[SlidingExpirationInSeconds] [bigint] NULL,
	[AbsoluteExpiration] [datetimeoffset](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
~~~

## Authors

* **Amit Kanti Barua** - *Remote Software Engineer* - [Amit Kanti Barua](https://github.com/tilamit) - *Built ReadME Template*

## Acknowledgements

* [Amit Kanti Barua](https://github.com/tilamit)
