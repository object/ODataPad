Feature: Service views

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

Scenario: Displaying collection information for a selected service
	Given I see a list of services
	When I select service OData.org
	Then I should see service collections
	| Name                      |
	| ODataConsumers            |
	| ODataProducerApplications |
	| ODataProducerLiveServices |

Scenario: Displaying collection properties
	Given I see a list of services
	And selected service is OData.org
	And collections are set to show its properties
	When I select collection ODataProducerApplications
	Then I should see collection schema summary "4 properties, 0 relations"

Scenario: Displaying collection data rows
	Given I see a list of services
	And selected service is OData.org
	And collections are set to show its data
	When I select collection ODataProducerApplications
	And I wait 5 seconds
	Then I should see collection data rows that contain
	| Key | Data                              |
	| 1   | SharePoint 2010 (...)             |
	| 2   | IBM WebSphere (...)               |
	| 4   | Windows Azure Table Storage (...) |

Scenario: Showing collection data details
	Given I see a list of services
	And selected service is OData.org
	And collections are set to show its data
	And selected collection is ODataProducerApplications
	When I select result row with key "4"
	Then I should see collection data details that contain
	| Name           | Data                                                |
	| Id             | 4                                                   |
	| Name           | Windows Azure Table Storage                         |
	| Description    | Windows Azure Table provides (...)                  |
	| ApplicationUrl | http://msdn.microsoft.com/en-us/azure/cc994380.aspx |

Scenario: Hiding collection data details
	Given I see a list of services
	And selected service is OData.org
	And collections are set to show its data
	And selected collection is ODataProducerApplications
	And collection data view shows collection data details for a row with key "4"
	When I tap within result view
	Then I should not see collection data details
	And I should see collection data rows that contain
	| Key | Data                              |
	| 1   | SharePoint 2010 (...)             |
	| 2   | IBM WebSphere (...)               |
	| 4   | Windows Azure Table Storage (...) |

