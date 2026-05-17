# Phase 2 Changes

## CI CD

- [x] Renew Azure Service Principle Secret For Az Dev Ops
- [ ] Convert to Federated Identity - removes secrets entirely and eliminates expiry issues.

## Service Bus
- [ ] Use a Azure Service Bus more with Subscription Filters which router filtered messages to different Azure Functions - Fan In / Fan Out scenario

## Logic App
- [ ] When a new journal is inserted into the Journals table, a SQL trigger fires.
- [ ] The Logic App retrieves the journal details, calls the User API to find the manager’s email, 
- [ ] and then uses the Outlook connector to send an email notification containing the journal information. This ensures managers are instantly notified whenever a developer submits a new journal

## BDD + SpecFlow
- [ ] Write BDD Test for Journal Creation using - Gherkin Syntax - Given, When, Then and SpecFlow lib