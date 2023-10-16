import { AzureFunction, Context, HttpRequest } from "@azure/functions";
import Openfort from "@openfort/openfort-node";

const openfort = new Openfort(process.env.OPENFORT_API_KEY);

const httpTrigger: AzureFunction = async function (
  context: Context,
  req: HttpRequest
): Promise<void> {
  try {
    if (
      !req.body ||
      !req.body.CallerEntityProfile.Lineage.MasterPlayerAccountId
    ) {
      context.res = {
        status: 400,
        body: "Please pass a valid request body",
      };
      return;
    }

    context.log("HTTP trigger function processed a request.");

    const OFplayer = await openfort.players
      .create({
        name: req.body.CallerEntityProfile.Lineage.MasterPlayerAccountId,
      })
      .catch((error) => {
        context.log(error);
        context.res = {
          status: 500,
          body: JSON.stringify(error),
        };
        return;
      });
    if (!OFplayer) return;

    const OFaccount = await openfort.accounts
      .create({
        player: OFplayer.id,
        chainId: 4337,
      })
      .catch((error) => {
        context.log(error);
        context.res = {
          status: 500,
          body: JSON.stringify(error),
        };
        return;
      });

    if (!OFaccount) return;

    context.log("API call was successful.");
    context.res = {
      status: 200,
      body: JSON.stringify({
        address: OFaccount.address,
        short_address:
          OFaccount.address?.substr(0, 5) +
          "..." +
          OFaccount.address?.substr(-4),
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
