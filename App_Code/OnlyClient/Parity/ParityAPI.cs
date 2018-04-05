using System;
using Nethereum.Web3;
using System.IO;
using System.Web;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Prem.PTC;
using Titan.Cryptocurrencies;

public class ParityAPI
{
    /// <summary>
    /// Returns Transaction Id
    /// </summary>
    /// <param name="senderAddress"></param>
    /// <param name="senderParityPassword"></param>
    /// <param name="destinationAddress"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static string SendTokens(string senderAddress, string senderParityPassword, string destinationAddress, decimal amount)
    {
        var TokenCryprocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);

        var TokenAmountAsInteger = new BigInteger(amount * TokenCryprocurrency.DecimalPlaces);

        var web3 = new Web3("http://localhost:8545");
        var targetContract = web3.Eth.GetContract(GetTokenAbi(), AppSettings.Ethereum.ERC20TokenContract);

        var balanceFunction = targetContract.GetFunction("balanceOf");
        var balanceFunctionAsyncCall = balanceFunction.CallAsync<BigInteger>("0x1480a8ddbab23081b88a404461333154ef961999");

        balanceFunctionAsyncCall.Wait();

        var availableBalance = balanceFunctionAsyncCall.Result;

        if (availableBalance < TokenAmountAsInteger)
            throw new MsgException("You don't have sufficient amount of tokens on this account.");

        //If you get 'Method not found' message, it means that you did not configured parity to run with RPC API: Personal
        //parity --warp --rpcapi "eth,net,web3,personal,parity"

        var unlockResult = web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, senderParityPassword, new HexBigInteger(120));
        unlockResult.Wait();

        var transferFunction = targetContract.GetFunction("transferFrom");
        var transferFunctionAsyncCall = transferFunction.SendTransactionAsync(
            senderAddress, new HexBigInteger(50000), new HexBigInteger(0),
            senderAddress, destinationAddress, TokenAmountAsInteger);

        transferFunctionAsyncCall.Wait();

        return transferFunctionAsyncCall.Result;
    }

    /// <summary>
    /// Returns amount of ERC20 tokens available in the transaction. If there are no tokens in the transaction, returns 0.
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    public static decimal GetTokensInTransaction(string transactionId)
    {
        //TODO
        //This method is not finished yet
        throw new NotImplementedException();

        decimal result = new decimal(0);

        try
        {
            var web3 = new Web3("http://localhost:8545");
            var targetContract = web3.Eth.GetContract(GetTokenAbi(), AppSettings.Ethereum.ERC20TokenContract);

            var getTransactionAsyncCall = web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionId);
            getTransactionAsyncCall.Wait();

            var transaction = getTransactionAsyncCall.Result;

            if (transaction.To != AppSettings.Ethereum.ERC20TokenContract)
                throw new MsgException("Transaction not sent to the token contract address.");

           
        }
        catch (Exception ex)
        { }

        return result;
    }

    private static string GetTokenAbi()
    {
        return @"
[
	{
		""constant"": true,

        ""inputs"": [],
		""name"": ""name"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""string""

            }
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": false,
		""inputs"": [
			{
				""name"": ""_spender"",
				""type"": ""address""
			},
			{
				""name"": ""_value"",
				""type"": ""uint256""
			}
		],
		""name"": ""approve"",
		""outputs"": [
			{
				""name"": ""success"",
				""type"": ""bool""
			}
		],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [],
		""name"": ""totalSupply"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""uint256""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": false,
		""inputs"": [
			{
				""name"": ""_from"",
				""type"": ""address""
			},
			{
				""name"": ""_to"",
				""type"": ""address""
			},
			{
				""name"": ""_value"",
				""type"": ""uint256""
			}
		],
		""name"": ""transferFrom"",
		""outputs"": [
			{
				""name"": ""success"",
				""type"": ""bool""
			}
		],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [],
		""name"": ""decimals"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""uint8""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [],
		""name"": ""version"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""string""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [
			{
				""name"": ""_owner"",
				""type"": ""address""
			}
		],
		""name"": ""balanceOf"",
		""outputs"": [
			{
				""name"": ""balance"",
				""type"": ""uint256""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [],
		""name"": ""symbol"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""string""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": false,
		""inputs"": [
			{
				""name"": ""_to"",
				""type"": ""address""
			},
			{
				""name"": ""_value"",
				""type"": ""uint256""
			}
		],
		""name"": ""transfer"",
		""outputs"": [
			{
				""name"": ""success"",
				""type"": ""bool""
			}
		],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""constant"": false,
		""inputs"": [
			{
				""name"": ""_spender"",
				""type"": ""address""
			},
			{
				""name"": ""_value"",
				""type"": ""uint256""
			},
			{
				""name"": ""_extraData"",
				""type"": ""bytes""
			}
		],
		""name"": ""approveAndCall"",
		""outputs"": [
			{
				""name"": ""success"",
				""type"": ""bool""
			}
		],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""constant"": true,
		""inputs"": [
			{
				""name"": ""_owner"",
				""type"": ""address""
			},
			{
				""name"": ""_spender"",
				""type"": ""address""
			}
		],
		""name"": ""allowance"",
		""outputs"": [
			{
				""name"": ""remaining"",
				""type"": ""uint256""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""inputs"": [],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""constructor""
	},
	{
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""fallback""
	},
	{
		""anonymous"": false,
		""inputs"": [
			{
				""indexed"": true,
				""name"": ""_from"",
				""type"": ""address""
			},
			{
				""indexed"": true,
				""name"": ""_to"",
				""type"": ""address""
			},
			{
				""indexed"": false,
				""name"": ""_value"",
				""type"": ""uint256""
			}
		],
		""name"": ""Transfer"",
		""type"": ""event""
	},
	{
		""anonymous"": false,
		""inputs"": [
			{
				""indexed"": true,
				""name"": ""_owner"",
				""type"": ""address""
			},
			{
				""indexed"": true,
				""name"": ""_spender"",
				""type"": ""address""
			},
			{
				""indexed"": false,
				""name"": ""_value"",
				""type"": ""uint256""
			}
		],
		""name"": ""Approval"",
		""type"": ""event""
	}
]";

    }
}