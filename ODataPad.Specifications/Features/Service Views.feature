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
	Then I should see collection properties
	| Name           | Type          |
	| Id             | Int32 (key)   |
	| Name           | String        |
	| Description    | String        |
	| ApplicationUrl | String (null) |

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
	And seleted collection data row is a row with key 4
	When I tap on selected collection data row
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
	And seleted collection data row is a row with key 4
	And collection data view shows collection data details
	When I tap on collection data view
	Then I should see collection data rows that contain
	| Key | Data                              |
	| 1   | SharePoint 2010 (...)             |
	| 2   | IBM WebSphere (...)               |
	| 4   | Windows Azure Table Storage (...) |

