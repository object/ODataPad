Feature: Services

Scenario: Initial screen should display a list of available services
	When I start the application
	Then I should see a list of services
	| Name               |
	| OData.org          |
	| Netflix            |
	| ebay               |
	| twitpic            |
	| Stack Overflow     |
	| Devexpress Channel |
	| Pluralsight        |
	| NuGet              |
	| nerddinner         |
	| Northwind Service  |
