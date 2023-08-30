# Project outline (plan)

## Microservices overview
- API: Handles incoming and outgoing HTTP requests
- Request processor (RP): Middleware that processes every request
- Configurator (CONF): Handles config management, keeps everything in sync
- Dashboard (DASH): Frontend for configurator, provides an overview of the system and management UI
- Common complex operations (CCO): Handles complex and stateful operations, a "standard library" for API development

## Architecture overview
![Architecture diagram (kind of)](https://cloud.luka-j.rocks/index.php/s/TQgRBBWZkafaXYe/preview)

## API
### Dependencies
**Makes calls to**:
- RP: for request processing
- CCO: for auth

**Is called by**:
- RP: for making requests to backends (always)
- CONF: for config management (always)
- Public

### gRPC Interface
- makeHttpRequest (takes HttpRequest, returns HttpResponse, TBD in conjuction with RP)
- updateFrontendConfig, updateBackendConfig (takes new config as string and start time)
- deleteFrontendConfig, deleteBackendConfig (takes config name, version and deletion time)
- getAllFrontendConfigs, getAllBackendConfigs (returns list of configs with metadata)
- getFrontendConfig, getBackendConfig (takes config name and version, returns config value as string)

## RP
### Dependencies
**Makes calls to**:
- API: for making http requests 
- CCO: for complex and stateful operations

**Is called by**:
- API: for request processing (always)
- CONF: for config management (always)

### gRPC Interface
- processRequest (takes HttpRequest, returns HttpResponse, TBD in conjuction with API)
- updateConfig (takes new config as string and start time)
- deleteConfig (takes config name and version)
- getConfig (takes config name and version, returns config value as string)

## CONF
### Dependencies
**Makes calls to**:
- API, RP, CCO: for config management

**Is called by**:
- DASH: for triggering operations (always)

**External dependencies**:
- Config storage: this can be Git, some cloud object storage, etc. (for development purposes, even local directory)
- User management system (e.g. Keycloak), for validating tokens
- ... ?

### HTTP endpoints
- getAllConfigs
- getConfig
- updateConfig (takes config, pushes it to storage; separate variations for API, RP, CCO)
- syncConfigs (pull from storage, push to appropriate microservice)
- deleteConfig (trigger call on appropriate microservice)
- ... ?
*Make variations for different config types where it makes sense.*

## DASH
### Dependencies
**Makes calls to**:
- CONF: for triggering operations

*Isn't called by anybody*

**External dependencies**:
- User management system (e.g. Keycloak), for issuing tokens

*Doesn't provide an API; SPA is available on a single endpoint.*

## CCO
### Dependencies
**Is called by**:
- API: for executing auth (often)
- RP: for executing stateful operations (often)
- CONF: for config management (always)

*Doesn't call anyone internally.*

**External dependencies**:
- User management system (for validating auth tokens)
- Database (cloud managed, maybe a KV store?)
- Cache (e.g. Redis)
- Queue
- ... ?

**gRPC Endpoints**:
- updateConfig (takes new config as string and start time)
- deleteConfig (takes config name and type)
- getAllConfigs (returns all configs / datasources)
- getConfig (takes config name and type, returns config value as string)
- performAuth (takes auth spec and token, returns success or auth error message)
- cacheStore (takes collection, key, value, TTL), cacheFetch (takes collection, key, returns value and TTL), cacheDelete (takes collection, key), cachePurge (takes collection)
- kvStore (takes collection, key, value), kvFetch (takes collection, key, returns value), kvDelete (takes collection, key), kvPurge (takes collection, key)
- DB operations ?
- Queue operations ?
- Other complex tasks ?
