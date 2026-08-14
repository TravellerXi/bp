# User story
#   As a user of the BP calculator
#   I want to see my mean arterial pressure alongside the category
#   So that I can tell whether my average perfusion pressure is adequate

Feature: Mean arterial pressure
  Mean arterial pressure is reported with the blood pressure category

  Scenario Outline: MAP is calculated from the systolic and diastolic readings
    Given a systolic reading of <systolic>
    And a diastolic reading of <diastolic>
    When I ask for the mean arterial pressure
    Then the mean arterial pressure should be <map>

    Examples:
      | systolic | diastolic | map   |
      | 120      | 80        | 93.3  |
      | 150      | 90        | 110.0 |
      | 90       | 60        | 70.0  |
      | 70       | 40        | 50.0  |

  Scenario Outline: MAP is placed in a perfusion band
    Given a systolic reading of <systolic>
    And a diastolic reading of <diastolic>
    When I ask for the mean arterial pressure band
    Then the mean arterial pressure band should be "<band>"

    Examples: Lower limits are inclusive
      | systolic | diastolic | band   |
      | 130      | 85        | High   |
      | 129      | 85        | Normal |
      | 90       | 60        | Normal |
      | 89       | 60        | Low    |

  Scenario: A reading that is ideal by category can still have a normal MAP
    Given a systolic reading of 110
    And a diastolic reading of 70
    When I ask for the blood pressure category
    Then the category should be "Ideal"
    When I ask for the mean arterial pressure band
    Then the mean arterial pressure band should be "Normal"
