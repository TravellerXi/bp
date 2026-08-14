# Gherkin DSL — acceptance criteria for the BP category calculator.
# The Scenario Outline values are the pairs published by the lecturer in the
# CA1 feedback announcement (Brightspace 148608, 06/01/2026).

Feature: Blood pressure category
  As a user of the BP calculator
  I want my systolic and diastolic readings classified
  So that I can see which blood pressure category I fall into

  Scenario Outline: Readings are classified into the correct category
    Given a systolic reading of <systolic>
    And a diastolic reading of <diastolic>
    When I ask for the blood pressure category
    Then the category should be "<category>"

    Examples: High
      | systolic | diastolic | category |
      | 150      | 90        | High     |
      | 140      | 40        | High     |
      | 95       | 90        | High     |

    Examples: Pre-High
      | systolic | diastolic | category |
      | 130      | 70        | PreHigh  |
      | 100      | 85        | PreHigh  |
      | 120      | 89        | PreHigh  |

    Examples: Ideal
      | systolic | diastolic | category |
      | 110      | 70        | Ideal    |
      | 90       | 75        | Ideal    |
      | 80       | 65        | Ideal    |

    Examples: Low
      | systolic | diastolic | category |
      | 89       | 50        | Low      |
      | 80       | 59        | Low      |
      | 70       | 40        | Low      |

  Scenario Outline: Lower limits of each band are inclusive
    Given a systolic reading of <systolic>
    And a diastolic reading of <diastolic>
    When I ask for the blood pressure category
    Then the category should be "<category>"

    Examples:
      | systolic | diastolic | category |
      | 140      | 60        | High     |
      | 139      | 60        | PreHigh  |
      | 100      | 90        | High     |
      | 100      | 89        | PreHigh  |
      | 120      | 60        | PreHigh  |
      | 119      | 60        | Ideal    |
      | 100      | 80        | PreHigh  |
      | 100      | 79        | Ideal    |

  Scenario: A single low reading is not enough to be classed as Low
    Given a systolic reading of 80
    And a diastolic reading of 65
    When I ask for the blood pressure category
    Then the category should be "Ideal"

  Scenario: Both readings below the ideal band are classed as Low
    Given a systolic reading of 89
    And a diastolic reading of 59
    When I ask for the blood pressure category
    Then the category should be "Low"
