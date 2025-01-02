const { app } = require('@azure/functions');
const { SearchIndexerClient, AzureKeyCredential } = require('@azure/search-documents');

require("dotenv").config();

const searchEndpoint = process.env.SEARCH_ENDPOINT;
const searchApiKey = process.env.SEARCH_API_KEY;
const indexerName = process.env.INDEXER_NAME;

app.eventGrid('paragoniarzEventGridTrigger', {
    handler: async (event, context) => {
        context.log('Event grid function processed event:', event);

        if (!searchEndpoint || !searchApiKey || !indexerName) {
            context.log.error("Make sure to set valid values for searchEndpoint and searchApiKey with proper authorization.");
            return;
        }

        if (!event.data) {
            context.log.error('Event data is missing, skipping processing.');
            return;
        }

        if (event.eventType === 'Microsoft.Storage.BlobCreated' || event.eventType === 'Microsoft.Storage.BlobDeleted') {
            try {
                context.log('Getting indexer client...');
                const indexerClient = new SearchIndexerClient(searchEndpoint, new AzureKeyCredential(searchApiKey));

                context.log('Run indexer:', indexerName);
                await indexerClient.runIndexer(indexerName);
                context.log('Indexer successfully run!');
            } catch (error) {
                context.log.error('Indexer failed:', error);
            }
        } else {
            context.log('Not supported event type:', event.eventType);
        }
    }
});