import { AzureFunction, Context, HttpRequest } from "@azure/functions";
import Openfort from "@openfort/openfort-node";

const openfort = new Openfort(process.env.OF_API_KEY);

const httpTrigger: AzureFunction = async function (
  context: Context,
  req: HttpRequest
): Promise<void> {
  context.log("Function triggered.");

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
    context.log("Valid request body found.");

    const playerId = req.body.FunctionArgument.playerId;
    const receiverAddress = req.body.FunctionArgument.receiverAddress;

    const player = await openfort.players
      .get({ id: playerId, expand: ["transactionIntents"] })
      .catch((error) => {
        context.log("Error while fetching player:", error);
        context.res = {
          status: 500,
          body: JSON.stringify(error),
        };
        return;
      });
    context.log("Player data fetched.");

    const transactionIntents = player["transactionIntents"];
    if (!transactionIntents || transactionIntents.length === 0) {
      context.log("No transaction intents found.");
      return;
    }
    context.log("Transaction intents found.");

    const interactions = await transactionIntents[0].interactions;
    if (!interactions || interactions.length !== 1) {
      context.log("Interactions check failed.");
      return;
    }
    context.log("Interaction check passed.");

    if (
      !interactions[0].functionName ||
      !interactions[0].functionName.includes("mint")
    ) {
      context.log("Function name check failed.");
      return;
    }
    context.log("Function name check passed.");

    let parsedAddress;

    try {
        parsedAddress = JSON.parse(interactions[0].functionArgs[0]);
    } catch (error) {
        context.log('Error parsing the address:', error);
        return;
    }

    if (!interactions[0].functionArgs || parsedAddress !== receiverAddress) {
        context.log(receiverAddress);
        context.log(parsedAddress);
        context.log("Receiver address check failed.");
        return;
    }
    context.log("Receiver address check passed.");

    context.res = {
      status: 200,
      body: JSON.stringify({
        id: transactionIntents[0].id
      }),
    };

    context.log("API call was successful.");
  } catch (error) {
    context.log("An unexpected error occurred:", error);
    context.res = {
      status: 500,
      body: JSON.stringify(error),
    };
  }
};

export default httpTrigger;
