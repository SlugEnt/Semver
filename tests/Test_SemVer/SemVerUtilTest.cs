﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.IO.Abstractions.TestingHelpers;
using NUnit.Framework;
using SlugEnt;
using SlugEnt.SemVer;

namespace Test_SemVer
{
	using XFS = MockUnixSupport;

	class SemVerUtilTest {
		private const string FIRST_VERSION = "0.0.4";
		private const string VERSION_2 = "1.2.5-alpha.14";
		private const string VERSION_3 = "1.2.5-alpha.a";
		private const string VERSION_4 = "1.2.5-beta.27";
		private const string VERSION_5 = "1.2.5-rc.2";
		private const string VERSION_6 = "1.2.5";
		private const string VERSION_7 = "1.2.6-fix.2";
		private const string VERSION_8 = "1.2.6-fix.3+somedata";
		private const string VERSION_9 = "1.2.6";
		private const string VERSION_10 = "1.2.15-beta.304";
		private const string VERSION_11= "1.2.15";
		private const string VERSION_12 = "1.3.0";
		private const string VERSION_13 = "2.6.104";
		private const string VERSION_14 = "3.0.0+somedata";
		private const string MAX_VERSION = "4.0.1";


		[SetUp]
		public void Setup()
		{ }



		/// <summary>
		/// Setup FileSystem
		/// </summary>
		/// <returns></returns>
		private MockFileSystem SetupFileSystem() {
			var fileSystem = new MockFileSystem();

			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + FIRST_VERSION);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_2);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_3);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_4);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_5);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_6);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_7);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_8);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_9);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_10);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_11);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_12);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_13);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + VERSION_14);
			fileSystem.Directory.CreateDirectory(@"C:\\Ver" + MAX_VERSION);
			
			fileSystem.AddFile(@"C:\Ver1.3.0\Ver.txt",new MockFileData("some data in a file"));
			return fileSystem;
		}


		private MockFileSystem SetupNoVersions () {
			var fileSystem = new MockFileSystem();
			return fileSystem;
		}



		[Test]
		public void MostRecentVersion () {
			var fileSystem = SetupFileSystem();

			SemVerUtil semVerUtil = new SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			FileSemVer fileSemVer = semVerUtil.DirectoryMostRecentVersion();
			Assert.AreEqual(MAX_VERSION,fileSemVer.Version,"A10:");
		}


		
		[Test]
		public void OldestVersion()
		{
			var fileSystem = SetupFileSystem();

			SemVerUtil semVerUtil = new SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			FileSemVer fileSemVer = semVerUtil.DirectoryOldestVersion();
			Assert.AreEqual(FIRST_VERSION, fileSemVer.Version, "A10:");
		}


		[Test]
		public void NewestNVersions ([Range(1,4,1)] int n) {
			var fileSystem = SetupFileSystem();

			SemVerUtil semVerUtil = new SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			List<FileSemVer> newest =  semVerUtil.DirectoryNewestVersions(n);
			Assert.AreEqual(n, newest.Count,"A10:  Incorrect number of items");

			for ( int i = 0; i < n; i++ ) {
				switch ( i ) {
					case 0:
						Assert.AreEqual(MAX_VERSION, newest [i++].Version, "A20:  Newest item is incorrect");
						break;
					case 1:
						Assert.AreEqual(VERSION_14, newest[i++].Version, "A30:  2nd newest item is incorrect");
						break;
					case 2:
						Assert.AreEqual(VERSION_13, newest[i++].Version, "A40:  3rd newest item is incorrect");
						break;
					case 3:
						Assert.AreEqual(VERSION_12, newest[i++].Version, "A50:  4th newest item is incorrect");
						break;
					case 4:
						Assert.AreEqual(VERSION_11, newest[i++].Version, "A60:  5th newest item is incorrect");
						break;
				}
			}
		}


		[Test]
		public void OldestNVersions([Range(0,4,1)] int n)
		{
			var fileSystem = SetupFileSystem();

			SemVerUtil semVerUtil = new SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			List<FileSemVer> oldest = semVerUtil.DirectoryOldestVersions(n);
			Assert.AreEqual(n, oldest.Count, "A10:  Incorrect number of items");

			for (int i = 0; i < n; i++)
			{
				switch (i)
				{
					case 0:
						Assert.AreEqual(FIRST_VERSION, oldest[i++].Version, "A20:  Oldest item is incorrect");
						break;
					case 1:
						Assert.AreEqual(VERSION_2, oldest[i++].Version, "A30:  2nd Oldest item is incorrect");
						break;
					case 2:
						Assert.AreEqual(VERSION_3, oldest[i++].Version, "A40:  3rd oldest item is incorrect");
						break;
					case 3:
						Assert.AreEqual(VERSION_4, oldest[i++].Version, "A50:  4th oldest item is incorrect");
						break;
					case 4:
						Assert.AreEqual(VERSION_5, oldest[i++].Version, "A60:  5th oldest item is incorrect");
						break;
				}
			}
		}

		/// <summary>
		/// Tests that null is returned any time there are no versions in the internal list.
		/// </summary>
		[Test]
		public void NoVersions () {
			var fileSystem = SetupNoVersions();

			SemVerUtil semVerUtil = new SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			FileSemVer fileSemVer = semVerUtil.DirectoryMostRecentVersion();
			Assert.IsNull(fileSemVer);

			fileSemVer = semVerUtil.DirectoryOldestVersion();
			Assert.IsNull(fileSemVer);

			List<FileSemVer> lists = semVerUtil.DirectoryNewestVersions(4);
			Assert.IsNull(lists);

			lists = semVerUtil.DirectoryOldestVersions(5);
			Assert.IsNull(lists);

			lists = semVerUtil.OldestWithMin(2);
			Assert.AreEqual(0,lists.Count);
		}



		[Test]
		public void KeepNNewVersions ([Range(0,15,1)] int n ) {
			var fileSystem = SetupFileSystem();

			SemVerUtil semVerUtil = new SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			int max = semVerUtil.VersionCount;

			List<FileSemVer> oldest = semVerUtil.OldestWithMin(n);
			Assert.AreEqual(max - n, oldest.Count, "A10:  Incorrect number of items");

			for (int i = 0; i < (max -n); i++)
			{
				switch ( i ) {
					case 0:
						Assert.AreEqual(FIRST_VERSION, oldest [i++].Version, "A20:  Oldest item is incorrect");
						break;
					case 1:
						Assert.AreEqual(VERSION_2, oldest [i++].Version, "A30:  2nd Oldest item is incorrect");
						break;
					case 2:
						Assert.AreEqual(VERSION_3, oldest [i++].Version, "A40:  3rd oldest item is incorrect");
						break;
					case 3:
						Assert.AreEqual(VERSION_4, oldest [i++].Version, "A50:  4th oldest item is incorrect");
						break;
					case 4:
						Assert.AreEqual(VERSION_5, oldest [i++].Version, "A60:  5th oldest item is incorrect");
						break;
					case 5:
						Assert.AreEqual(VERSION_6, oldest [i++].Version, "A60:  6th oldest item is incorrect");
						break;
					case 6:
						Assert.AreEqual(VERSION_7, oldest [i++].Version, "A60:  7th oldest item is incorrect");
						break;
					case 7:
						Assert.AreEqual(VERSION_8, oldest [i++].Version, "A60:  8th oldest item is incorrect");
						break;
					case 8:
						Assert.AreEqual(VERSION_9, oldest [i++].Version, "A60:  9th oldest item is incorrect");
						break;
					case 9:
						Assert.AreEqual(VERSION_10, oldest [i++].Version, "A60:  10th oldest item is incorrect");
						break;
					case 10:
						Assert.AreEqual(VERSION_11, oldest [i++].Version, "A60:  11th oldest item is incorrect");
						break;
					case 11:
						Assert.AreEqual(VERSION_12, oldest [i++].Version, "A60:  12th oldest item is incorrect");
						break;
					case 12:
						Assert.AreEqual(VERSION_13, oldest [i++].Version, "A60:  13th oldest item is incorrect");
						break;
					case 13:
						Assert.AreEqual(VERSION_14, oldest [i++].Version, "A60:  14th oldest item is incorrect");
						break;
					case 14:
						Assert.AreEqual(MAX_VERSION, oldest [i++].Version, "A60:  15th oldest item is incorrect");
						break;
					case 15:
						Assert.AreEqual(0, oldest.Count);
						break;
				}
			}
		}


		[TestCase(32)]
		[TestCase(20)]
		[Test]
		public void KeepNNewVersionsToMany (int n) {
			var fileSystem = SetupFileSystem();

			SemVerUtil semVerUtil = new SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			int max = semVerUtil.VersionCount;
			Assert.Greater(n,max,"A10:");
			List<FileSemVer> oldest = semVerUtil.OldestWithMin(n);
			Assert.AreEqual(0,oldest.Count,  "A20:  Incorrect number of items");
		}


		[Test]
		public void KeepNNewVersionsTimeStamp () {
			// Setup
			TimeUnit day2plus3hr = new TimeUnit("2d");
			day2plus3hr.AddHours(3);

			var fileSystem = new MockFileSystem();
			DateTime d5 = DateTime.Now.AddDays(-5);
			DateTime d4 = DateTime.Now.AddDays(-4);
			DateTime d3 = DateTime.Now.AddDays(-3);
			DateTime d2 = DateTime.Now.AddSeconds(day2plus3hr);
			DateTime d1 = DateTime.Now.AddDays(-1);
			DateTime d020 = DateTime.Now.AddHours(-20);
			DateTime d010 = DateTime.Now.AddHours(-10);
			DateTime d04 = DateTime.Now.AddHours(-4);
			DateTime d02 = DateTime.Now.AddHours(-2);


			AddVerDirectory(fileSystem, FIRST_VERSION, d5);
			AddVerDirectory(fileSystem, VERSION_2, d4);
			AddVerDirectory(fileSystem, VERSION_3, d3);
			AddVerDirectory(fileSystem, VERSION_4, d2);
			AddVerDirectory(fileSystem, VERSION_5, d1);
			AddVerDirectory(fileSystem, VERSION_6, d020);
			AddVerDirectory(fileSystem, VERSION_7, d010);
			AddVerDirectory(fileSystem, VERSION_8, d04);
			AddVerDirectory(fileSystem, VERSION_9, d02);

			fileSystem.AddFile(@"C:\Ver1.3.0\Ver.txt", new MockFileData("some data in a file"));

			// Test
			SemVerUtil semVerUtil = new SemVerUtil(fileSystem);
			semVerUtil.Initialize(@"C:\", "Ver");

			int max = semVerUtil.VersionCount;
			int minToKeep = 3;
			
			List<FileSemVer> oldest = semVerUtil.OldestWithMinAge(minToKeep, new TimeUnit("2d"));
			Assert.AreEqual(3, oldest.Count, "A10:  Incorrect number of items");
			Assert.AreEqual(FIRST_VERSION, oldest[0].Version, "A20:  Oldest item is incorrect");
			Assert.AreEqual(VERSION_2, oldest[1].Version, "A30:  item is incorrect");
			Assert.AreEqual(VERSION_3, oldest[2].Version, "A40:  item is incorrect");
		}


		private void AddVerDirectory (MockFileSystem fileSystem, string nameSuffix, DateTime timeStamp) {
			string name = @"C:\\Ver" + nameSuffix;
			fileSystem.Directory.CreateDirectory(name);
			fileSystem.Directory.SetCreationTime(name,timeStamp);
		}


		[Test]
		public void InvalidPathThrows () {
			MockFileSystem fileSystem = SetupFileSystem();

			SemVerUtil semverUtil = new SemVerUtil(fileSystem);
			;
			Assert.Throws<ArgumentException>(() => semverUtil.Initialize(@"C:\dummPath", ""));
		}


		[Test]
		public void NotInitializedThrowsInDirectoryMostRecent()
		{
			MockFileSystem fileSystem = SetupFileSystem();
			SemVerUtil semverUtil = new SemVerUtil(fileSystem);
			
			Assert.Throws<ApplicationException>(() => semverUtil.DirectoryMostRecentVersion());
		}


		[Test]
		public void NotInitializedThrowsInNewest()
		{
			MockFileSystem fileSystem = SetupFileSystem();
			SemVerUtil semverUtil = new SemVerUtil(fileSystem);

			Assert.Throws<ApplicationException>(() => semverUtil.DirectoryNewestVersions(2));
		}



		[Test]
		public void NotInitializedThrowsInOldestVersion()
		{
			MockFileSystem fileSystem = SetupFileSystem();
			SemVerUtil semverUtil = new SemVerUtil(fileSystem);

			Assert.Throws<ApplicationException>(() => semverUtil.DirectoryOldestVersion());
		}





		[Test]
		public void NotInitializedThrowsInOldestVersions()
		{
			MockFileSystem fileSystem = SetupFileSystem();
			SemVerUtil semverUtil = new SemVerUtil(fileSystem);

			Assert.Throws<ApplicationException>(() => semverUtil.DirectoryOldestVersions(2));
		}





		[Test]
		public void NotInitializedThrowsInOldestWithMin()
		{
			MockFileSystem fileSystem = SetupFileSystem();
			SemVerUtil semverUtil = new SemVerUtil(fileSystem);

			Assert.Throws<ApplicationException>(() => semverUtil.OldestWithMin(2));
		}





		[Test]
		public void DirectoryOldestCountLessThanKeep()
		{
			MockFileSystem fileSystem = SetupFileSystem();
			SemVerUtil semverUtil = new SemVerUtil(fileSystem);
			semverUtil.Initialize(@"C:\","Ver");
			int versionCount = fileSystem.Directory.GetDirectories(@"C:\", "Ver*").Length;


			// This is requesting we keep 25 versions, but we only have 15 in directory...
			List<FileSemVer> versions = semverUtil.DirectoryOldestVersions(25);
			Assert.AreEqual(versionCount,versions.Count);
		}




		[Test]
		public void DirectoryNewestCountLessThanKeep()
		{
			MockFileSystem fileSystem = SetupFileSystem();
			SemVerUtil semverUtil = new SemVerUtil(fileSystem);
			semverUtil.Initialize(@"C:\", "Ver");
			int versionCount = fileSystem.Directory.GetDirectories(@"C:\", "Ver*").Length;


			// This is requesting we keep 25 versions, but we only have 15 in directory...
			List<FileSemVer> versions = semverUtil.DirectoryNewestVersions(25);
			Assert.AreEqual(versionCount, versions.Count);
		}
	}
}
