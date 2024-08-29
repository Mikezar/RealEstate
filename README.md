# RealEstate

## Overview

The project is divided into 3 main parts: Console, Domain, and Application.

**Console** is only responsible for setting up dependencies and dispatching calls. It could be easier to create direct instances of the objects, but the solution is hard to maintain and makes it less portable, if the logic is required to be used in Web API project, for example. For the ease of debugging, I used the preprocessor directive.
When running the application via cmd, **a search query**, which is the location of the search area, must be provided.
Configuration for external API is stored in the configuration file.

**Domain** is a tiny project, as it doesn't have many concepts, but its idea is to set the boundaries and make distinguish between the external objects we receive and internally used for the needs of the application.

**Application** includes all important logic of the application.

## Implementation details

I used the Console type of application, because the task to get the top X of real estate agents falls into async type of jobs, for example, for making reports. This type of operation is hard to process in real-time, due to the timing and the API request limit (we need to rate-limit, consequently, it takes even more time). 
```
using (var scope = provider.CreateScope())
```
I am creating all services as a part of the scope, as it won't cause any issues with http connections and port availabilities, especially, if the applciation needs to work continuously and process multiple requests.

**FundaGateway** service is responsible for making and handling calls to Funda API.

**FundaQueryBuilder** translates search options into query string to meet Funda API specification. Having this as a separate injection helps to test the logic without the need to handle the complications of httpClient in tests.

**FundaGatewayDecorator** could be part of **FundaGateway**, but keeping extra logic aside, that can be extended or changed is a better way. Decorator adds rate-liming.

**FundaBrokerAdapter** is more an anti-corruption layer, that converts data from Funda to our domain concepts, also working with Funda API to fetch the required data in the needed format. 
As we need to get top 10 of real estate agents with the most objects, then we need to fetch all the data and aggregate them, so that we could process it further in the next step. 
If the service gets a 401 error message, it will end the execution of the loop immediately, as it means the request limit was exceeded, which should not be the case as we use Polly, but the error may still occur due to external factors (for example, the key is used in other applications, limit is changed).

**BrokerService** is a high-level service, that does final aggregation. If something is changed in Funda, it would only affect the gateway and adapter service.

## Important to know

Polly pipeline rate-limits the calls to API based on the token bucket algorithm, once it reaches 100, it uses the queue to put not squeezed calls there, to process later. It means, that the program may take more than a minute to process all data not to exceed the set limit.

Unfortunately, changing the parameter of **pagesize** to a different number doesn't affect the size of returned data.

In case of exceeding the allowed number of requests per minute, Funda API seems to return 401 status code, instead of expected 429.

I didn't take into account the possible overlapping /duplication of data between the pages (due to its dynamic change) so as not to overcomplicate the solution.
