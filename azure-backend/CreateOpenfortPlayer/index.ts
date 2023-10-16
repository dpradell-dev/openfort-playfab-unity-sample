import { AzureFunction, Context, HttpRequest } from "@azure/functions";
import Openfort from "@openfort/openfort-node";

const OPENFORT_API_KEY = process.env.OPENFORT_API_KEY;
const CHAIN_ID = 80001; //Mumbai

if (!OPENFORT_API_KEY) {
    throw new Error("OPENFORT_API_KEY not set in environment variables.");
}

const openfort = new Openfort(OPENFORT_API_KEY);

const httpTrigger: AzureFunction = async function (
  context: Context,
  req: HttpRequest
): Promise<void> {
    try {
        validateRequestBody(req);

        context.log("Creating player in Openfort...");
        const OFplayer = await createOpenfortPlayer(req.body.CallerEntityProfile.Lineage.MasterPlayerAccountId);

        context.log("Creating account in Openfort...");
        const OFaccount = await createOpenfortAccount(OFplayer.id);

        context.res = buildSuccessResponse(OFaccount);
    } catch (error) {
        context.log(error);
        context.res = {
            status: 500,
            body: JSON.stringify(error),
        };
    }
};

function validateRequestBody(req: HttpRequest): void {
    if (!req.body || !req.body.CallerEntityProfile.Lineage.MasterPlayerAccountId) {
        throw new Error("Please pass a valid request body");
    }
}

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

function buildSuccessResponse(OFaccount: any) {
    const address = OFaccount.address;
    const short_address = `${address?.substr(0, 5)}...${address?.substr(-4)}`;

    return {
        status: 200,
        body: JSON.stringify({
            address,
            short_address
        })
    };
}

export default httpTrigger;