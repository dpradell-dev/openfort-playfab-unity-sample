import { AzureFunction, Context, HttpRequest } from "@azure/functions";
import Openfort from "@openfort/openfort-node";

const OF_API_KEY = process.env.OF_API_KEY;
const CHAIN_ID = 80001; // Mumbai

if (!OF_API_KEY) {
    throw new Error("OF_API_KEY not set in environment variables.");
}

const openfort = new Openfort(OF_API_KEY);

function validateRequestBody(req: HttpRequest): void {
    if (!req.body || !req.body.CallerEntityProfile.Lineage.MasterPlayerAccountId) {
        throw new Error("Invalid request body: Missing required parameters.");
    }
}

const httpTrigger: AzureFunction = async function (
  context: Context,
  req: HttpRequest
): Promise<void> {
    context.log("Starting HTTP trigger function processing.");

    try {
        validateRequestBody(req);

        context.log("Creating player in Openfort...");
        const OFplayer = await createOpenfortPlayer(req.body.CallerEntityProfile.Lineage.MasterPlayerAccountId);

        context.log(`Player with ID ${OFplayer.id} created. Proceeding to create account in Openfort...`);
        const OFaccount = await createOpenfortAccount(OFplayer.id);

        context.log(`Account with address ${OFaccount.address} created.`);
        context.res = buildSuccessResponse(OFplayer.id, OFaccount.address);
        context.log("Function execution successful and response sent.");
    } catch (error) {
        context.log("An error occurred:", error);
        context.res = {
            status: 500,
            body: JSON.stringify(error),
        };
    }
};

async function createOpenfortPlayer(masterPlayerAccountId: string) {
  const OFplayer = await openfort.players.create({ name: masterPlayerAccountId });
  
  if (!OFplayer) {
      throw new Error("Failed to create Openfort player.");
  }
  return OFplayer;
}

async function createOpenfortAccount(playerId: string) {
  const OFaccount = await openfort.accounts.create({
      player: playerId,
      chainId: CHAIN_ID,
  });

  if (!OFaccount) {
      throw new Error("Failed to create Openfort account.");
  }
  return OFaccount;
}

function buildSuccessResponse(OFplayerId: string, OFaccountAddress: string) {
    return {
        status: 200,
        body: JSON.stringify({
            playerId: OFplayerId,
            playerWalletAddress: OFaccountAddress
        })
    };
}

export default httpTrigger;