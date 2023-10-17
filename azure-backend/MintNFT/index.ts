import { AzureFunction, Context, HttpRequest } from "@azure/functions";
import Openfort, {
    CreateTransactionIntentRequest,
    Interaction
} from "@openfort/openfort-node";

const OF_API_KEY = process.env.OF_API_KEY;
const CHAIN_ID = 80001; //Mumbai
const OF_NFT_CONTRACT = process.env.OF_NFT_CONTRACT;
const OF_SPONSOR_POLICY = process.env.OF_SPONSOR_POLICY;

if (!OF_API_KEY || !OF_NFT_CONTRACT || !OF_SPONSOR_POLICY) {
    throw new Error("Required environment variables are not set.");
}

const openfort = new Openfort(OF_API_KEY);

const httpTrigger: AzureFunction = async function (
    context: Context,
    req: HttpRequest
): Promise<void> {
    try {
        validateRequestBody(req);

        const { playerId, receiverAddress } = req.body.FunctionArgument;
        const transactionIntent = await createTransactionIntent(playerId, receiverAddress);

        context.res = buildSuccessResponse(transactionIntent.id);
    } catch (error) {
        context.log(error);
        context.res = {
            status: 500,
            body: JSON.stringify(error),
        };
    }
};

function validateRequestBody(req: HttpRequest): void {
    if (!req.body || 
        !req.body.CallerEntityProfile.Lineage.MasterPlayerAccountId ||
        !req.body.FunctionArgument.playerId ||
        !req.body.FunctionArgument.receiverAddress) {
        throw new Error("Please pass a valid request body");
    }
}

async function createTransactionIntent(playerId: string, receiverAddress: string): Promise<any> {
    const interaction: Interaction = {
        contract: OF_NFT_CONTRACT,
        functionName: "mint",
        functionArgs: [receiverAddress]
    };

    const transactionIntentRequest: CreateTransactionIntentRequest = {
        player: playerId,
        chainId: CHAIN_ID,
        optimistic: false,
        interactions: [interaction],
        policy: OF_SPONSOR_POLICY
    };

    const transactionIntent = await openfort.transactionIntents.create(transactionIntentRequest);

    if (!transactionIntent) {
        throw new Error("Failed to create transaction intent.");
    }
    return transactionIntent;
}

function buildSuccessResponse(transactionIntentId: string) {
    return {
        status: 200,
        body: JSON.stringify({
            id: transactionIntentId
        })
    };
}

export default httpTrigger;
