import { AzureFunction, Context, HttpRequest } from "@azure/functions";
import Openfort, {
  CreateTransactionIntentRequest,
  Interaction,
} from "@openfort/openfort-node";

const openfort = new Openfort(process.env.OPENFORT_API_KEY);
const CHAIN_ID = 80001; //Mumbai

const httpTrigger: AzureFunction = async function (
  context: Context,
  req: HttpRequest
): Promise<void> {
  try {
    if (
      !req.body ||
      !req.body.CallerEntityProfile.Lineage.MasterPlayerAccountId ||
      !req.body.FunctionArgument.playerId ||
      !req.body.FunctionArgument.receiverAddress
    ) {
      context.res = {
        status: 400,
        body: "Please pass a valid request body",
      };
      return;
    }

    const playerId = req.body.FunctionArgument.playerId;
    const receiverAddress = req.body.FunctionArgument.receiverAddress;

    const interaction_1: Interaction = {
      contract: process.env.OF_NFT_CONTRACT,
      functionName: "mint",
      functionArgs: [receiverAddress],
    };

    const transactionIntentRequest: CreateTransactionIntentRequest = {
      player: playerId,
      chainId: CHAIN_ID,
      optimistic: false,
      interactions: [interaction_1],
      policy: process.env.OF_TX_SPONSOR,
    };
    const transactionIntent = await openfort.transactionIntents.create(
      transactionIntentRequest
    );

    if (!transactionIntent) return;

    context.log("API call was successful.");
    context.res = {
      status: 200,
      body: JSON.stringify({
        id: transactionIntent.id
      }),
    };
  } catch (error) {
    context.log(error);
    context.res = {
      status: 500,
      body: JSON.stringify(error),
    };
  }
};

export default httpTrigger;
