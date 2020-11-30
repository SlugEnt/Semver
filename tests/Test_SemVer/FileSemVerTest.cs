using System;
using System.Collections.Generic;
using System.IO.Abstractions;

using Moq;
using NUnit.Framework;
using SemVerUtil;


namespace Test_SemVer
{
	

	public class FileSemVerTest {

		[SetUp]
		public void Setup() {

		}



		[Test]
		public void NullNameThrows() {
			string nullString = null;
			string prefix = "pre";

			Assert.Throws < ArgumentException>(() => new FileSemVer(nullString, prefix));
		}


		[Test]
		public void EmptyNameThrows () {
			string nostring = null;
			string prefix = "pre";

			Assert.Throws<ArgumentException>(() => new FileSemVer(nostring, prefix));
		}


		
		[TestCase("Ver1.0.0", "Ver", "1.0.0")]
		[TestCase("Ver1.2.3", "Ver", "1.2.3")]
		[TestCase("Ver1.2.3-alpha.1", "Ver", "1.2.3-alpha.1")]
		[TestCase("Ver1.2.3-alpha.5009", "Ver", "1.2.3-alpha.5009")]
		[TestCase("Ver2.3.4-beta.3456+data", "Ver", "2.3.4-beta.3456+data")]
		[TestCase("Ver2.3.4-rc.25", "Ver", "2.3.4-rc.25")]
		[TestCase("Ver2.3.4-fix.somefix.103", "Ver", "2.3.4-fix.somefix.103")]

		[Test]
		public void ValidConstructs (string name, string prefix, string expectedSemVer) {
			FileSemVer fileSemVer = new FileSemVer(name,prefix);

			Assert.AreEqual( name ,fileSemVer.FileName,"A10:");
			Assert.AreEqual(prefix,fileSemVer.Prefix,"A20:");
			Assert.AreEqual(expectedSemVer,fileSemVer.SemVersion.ToString(), "A30:");

		}
	}
}