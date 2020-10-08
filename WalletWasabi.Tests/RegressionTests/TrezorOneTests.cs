using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WalletWasabi.Backend;
using WalletWasabi.Hwi;
using WalletWasabi.Hwi.Models;
using WalletWasabi.Tests.XunitConfiguration;
using Xunit;

namespace WalletWasabi.Tests.RegressionTests
{
	[Collection("RegTest collection")]
	public class TrezorOneTests
	{
		public TrezorOneTests(RegTestFixture regTestFixture)
		{
			RegTestFixture = regTestFixture;
		}

		private RegTestFixture RegTestFixture { get; }

		[Fact]
		public async Task Test()
		{
			var client = new HwiClient(RegTestFixture.BackendRegTestNode.Network);
			using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
			var trezor = (await client.EnumerateAsync(cts.Token))
				.FirstOrDefault(x => x.Model == HardwareWalletModels.Trezor_1);
			Assert.NotNull(trezor);
		}
	}
}
