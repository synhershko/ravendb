﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using Microsoft.Isam.Esent.Interop;
using Raven.Abstractions;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Abstractions.Linq;
using Raven.Abstractions.Logging;
using Raven.Abstractions.MEF;
using Raven.Client.Document;
using Raven.Database;
using Raven.Database.Config;
using Raven.Database.Impl;
using Raven.Database.Plugins;
using Raven.Database.Storage;
using Raven.Json.Linq;
using Raven.Tests.Bugs;
using Raven.Tests.Document;
using Raven.Tests.Faceted;
using Raven.Tests.Issues;
using System.Linq;
using Raven.Tests.Util;
using Xunit;
using Version = Lucene.Net.Util.Version;

namespace Raven.Tryouts
{
	internal class Program
	{
		[STAThread]
		private static void Main()
		{
			var now = SystemTime.UtcNow;
			var oa = now.ToOADate();
			var test = DateTime.FromBinary((long)oa);
			Console.WriteLine(now.ToString("o"));
			Console.WriteLine(test.ToString("o"));
				
		}
	}
}