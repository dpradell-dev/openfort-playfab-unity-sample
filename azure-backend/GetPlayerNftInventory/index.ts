import { AzureFunction, Context, HttpRequest } from "@azure/functions";
import Openfort from "@openfort/openfort-node";

const CHAIN_ID = 80001; // Mumbai
const openfort = new Openfort(process.env.OF_API_KEY);

function isValidRequestBody(body: any): boolean {
  return body &&
    body.CallerEntityProfile &&
    body.CallerEntityProfile.Lineage &&
    body.CallerEntityProfile.Lineage.MasterPlayerAccountId &&
    body.FunctionArgument &&
    body.FunctionArgument.playerId;
}

const httpTrigger: AzureFunction = async function (
  context: Context,
  req: HttpRequest
): Promise<void> {
  try {
    context.log("Starting HTTP trigger function processing.");

    if (!isValidRequestBody(req.body)) {
      context.log("Invalid request body received.");
      context.res = {
        status: 400,
        body: "Please pass a valid request body",
      };
      return;
    }

    const playerId = req.body.FunctionArgument.playerId;

    async function getPlayerNftInventory(playerId: string) {
      const inventory = await openfort.inventories.getPlayerNftInventory({ playerId: playerId, chainId: CHAIN_ID });
      
      if (!inventory) {
          throw new Error("Failed to retrieve inventory.");
      }
      return inventory;
    }

    // Call the function to get the player's NFT inventory
    const inventoryResponse = await getPlayerNftInventory(playerId);

    // Extract the 'data' section from the response
    const inventoryData = inventoryResponse.data;

    context.res = {
      status: 200,
      body: JSON.stringify(inventoryData),
    };

    context.log("API call was successful and response sent.");
  } catch (error) {
    context.log("Unhandled error occurred:", error);
    context.res = {
      status: 500,
      body: JSON.stringify(error),
    };
  }
};

export default httpTrigger;
