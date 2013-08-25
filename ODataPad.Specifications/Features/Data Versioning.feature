Feature: Data Versioning

Scenario: On the first run services should be created from samples
	Given Service repository has no entries
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

Scenario: On the upgrade new service samples should appear in the service list
	Given Service repository was created by the previous program version
	And Service repository has following entries
	| Name      |
	| OData.org |
	When I start the application
	Then I should see a list of services
	| Name               |
	| OData.org          |
	| Pluralsight        |

Scenario: On the upgrade expired service samples should disappear from the service list
	Given Service repository was created by the previous program version
	And Service repository has following entries
	| Name      |
	| OData.org |
	| DBpedia   |
	When I start the application
	Then I should see a list of services
	| Name               |
	| OData.org          |
	| Pluralsight        |

Scenario: On the upgrade user-defined services should stay in the service list
	Given Service repository was created by the previous program version
	And Service repository has following entries
	| Name       |
	| OData.org  |
	| MyData.com |
	When I start the application
	Then I should see a list of services
	| Name        |
	| OData.org   |
	| MyData.com  |
	| Pluralsight |

Scenario: On the upgrade user-deleted sample services should disappear from the service list
	Given Service repository was created by the previous program version
	And Service repository has following entries
	| Name      |
	| OData.org |
	When I start the application
	Then I should see a list of services
	| Name               |
	| OData.org          |
	| Pluralsight        |
