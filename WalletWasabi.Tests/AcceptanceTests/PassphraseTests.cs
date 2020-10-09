using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using WalletWasabi.Backend;
using WalletWasabi.Blockchain.Keys;
using WalletWasabi.Hwi;
using WalletWasabi.Hwi.Models;
using WalletWasabi.Tests.XunitConfiguration;
using Xunit;

namespace WalletWasabi.Tests.AcceptanceTests
{
	public class PassphraseTests
	{
		[Fact]
		public async Task Trezor_One()
		{
			var client = new HwiClient(Network.Main);
			using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
			var trezor = (await client.EnumerateAsync(cts.Token))
				.FirstOrDefault(x => x.Model == HardwareWalletModels.Trezor_1);
			Assert.NotNull(trezor);
			var t = trezor.Model;
			var p = trezor.Path;
			if (trezor.NeedsPinSent is true)
			{
				await client.PromptPinAsync(trezor.Model, trezor.Path, cts.Token);
				var scrambledPin = 1234;
				// BREAKPOINT and set scrambledPin
				/*
				 * 7 8 9
				 * 4 5 6
				 * 1 2 3
				 */
				await client.SendPinAsync(trezor.Model, trezor.Path, scrambledPin, cts.Token);
				trezor = (await client.EnumerateAsync(cts.Token))
					.FirstOrDefault(x => x.Model == HardwareWalletModels.Trezor_1);
				Assert.NotNull(trezor);
			}
			Assert.True(trezor.NeedsPassphraseSent);
			trezor = (await client.SendPassAsync(trezor.Model, trezor.Path, "p", cts.Token))
				.FirstOrDefault(x => x.Model == HardwareWalletModels.Trezor_1);
			Assert.NotNull(trezor);
			var f = trezor.Fingerprint;
			Assert.NotNull(f);

			var addr0 = await client.DisplayAddressAsync(f.Value, new KeyPath($"{KeyManager.DefaultAccountKeyPath}/0/0"), cts.Token);
			Assert.NotNull(addr0);
			var addr1 = await client.DisplayAddressAsync(t, p, new KeyPath($"{KeyManager.DefaultAccountKeyPath}/0/0"), cts.Token);
			Assert.NotNull(addr1);
			Assert.Equal(addr0, addr1);

			var xpub = await client.GetXpubAsync(t, p, KeyManager.DefaultAccountKeyPath, cts.Token);
			Assert.NotNull(xpub);
			var enumerate = await client.EnumerateAsync(cts.Token);
			var xpub2 = await client.GetXpubAsync(t, p, KeyManager.DefaultAccountKeyPath, cts.Token);
			Assert.NotNull(xpub2);
			Assert.Equal(xpub, xpub2);

			var addr2 = await client.DisplayAddressAsync(t, p, new KeyPath($"{KeyManager.DefaultAccountKeyPath}/0/0"), cts.Token);
			Assert.NotNull(addr2);
			Assert.Equal(addr1, addr2);
		}
	}
}
