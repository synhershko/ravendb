﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Linq;
using Xunit;

namespace Raven.Tests.Issues
{
	public class RavenDB_1472 : RavenTest
	{
		public class Cat
		{
			public string Id { get; set; }
			public string Name { get; set; }
		}

		[Fact]
		public void Multiple_terms_queries_with_AND_operator_should_return_intersection_of_results()
		{
			using (var store = NewDocumentStore())
			{
				using (var session = store.OpenSession())
				{
					session.Store(new Cat(), "cats/1");
					session.Store(new Cat(), "cats/2");
					session.Store(new Cat(), "cats/3");
					session.SaveChanges();
				}

				using (var session = store.OpenSession())
				{
					var cats = session.Query<Cat>()
						.Customize(q => q.WaitForNonStaleResultsAsOfLastWrite())
						.Where(cat => cat.Id.In("cats/1", "cats/2") && cat.Id.In("cats/2", "cats/3"))
						.ToList();

					Assert.Equal(1,cats.Count);
					Assert.Equal("cats/2",cats.First().Id);
				}
			}
		}

		[Fact]
		public void Multiple_terms_queries_with_OR_operator_should_return_union_of_results()
		{
			using (var store = NewDocumentStore())
			{
				using (var session = store.OpenSession())
				{
					session.Store(new Cat(), "cats/1");
					session.Store(new Cat(), "cats/2");
					session.Store(new Cat(), "cats/3");
					session.SaveChanges();
				}

				using (var session = store.OpenSession())
				{
					var cats = session.Query<Cat>()
						.Customize(q => q.WaitForNonStaleResultsAsOfLastWrite())
						.Where(cat => cat.Id.In("cats/1", "cats/2") || cat.Id.In("cats/2", "cats/3"))
						.ToList();

					Assert.Equal(3, cats.Count);
					Assert.Equal(cats.Select(row => row.Id), new[] { "cats/1", "cats/2", "cats/3" });
				}
			}
		}

	}
}