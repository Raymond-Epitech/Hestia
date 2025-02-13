# Beta Test Plan

## 1. Introduction
Hestia is a mobile app that requires beta-testing to ensure that it's functionalities corresponds to it's user base and their needs.
---

## 2. Selection of core functionalities
Identify and describe the core functionalities that will be available in the beta version. These should be based on the functional specifications from your Action Plan and refined based on project evolution, feedback, and new requirements.

### 2.1 Core functionalities
- **Account creation**: The ability to create a Hestia user account by connecting through a google account.
- **Account configuration**: Creating or joining an existing collocation, setting personal information.
- **Wall page**: Publish posts for other users in the same collocation to see, see other users posts.
- **Tasks page**: Set tasks and their deadlines, set the people assigned to said task.
- **Settings page**: Change the language of the app, the notification prefferences, send a bug report or a feature request/modification.


### 2.2 Adaptations and refinements
- None yet, app not yet beta tested.

---

## 3. Definition of beta testing scenarios
Develop structured test scenarios to assess the functionality of key features. Each scenario should include the user role, feature being tested, expected outcome, and steps to execute the test.

### 3.1 Test scenario 1
- **End-user**
- **Connection**
- **Precondition**:
  1. The user need to launch the application
- **User was able to connect**
- **Steps to execute**:
  1. User touches google connection button
  2. User connects to their google account through google's own login page
  3. User confirms the connection of the account

### 3.2 Test scenario 2
- **End-user**
- **Add a post**
- **Precondition**:
  1. The user is logged in
- **Post is added and is readable by other accounts in the same collocation**: 
- **Steps to execute**:
  1. User open the app
  2. User goes to the post page by touching the fridge icon
	3. User touches the “+” button
	4. An input form appears, allowing the user to fill in the post content and color
	5. User fill the blank and click on “poster”
	6. the post is added to the page

### 3.3 Test scenario 3
- **End-user**
- **Delete a post**
- **Precondition**:
  1. The user is logged in.
- **Post is deleted and the page refreshes**:
- **Steps to execute**:
  1. User opens the app.
  2. User goes to the post page by touching the fridge icon
  3. User touches the “x” button next to the post they want to delete
  4. The post is deleted, and the page refreshes

### 3.4 Test scenario 4
- **End-user**
- **Go to settings page**
- **Precondition**:
  1. The user is logged in
- **User is redirected to the settings page**:
- **Steps to execute**:
  1. User opens the app
  2. User touches the cog icon
  3. User is redirected to the settings page

### 3.5 Test scenario 5
- **End-user**
- **Go to money page**
- **Precondition**:
  1. The user is logged in
- **User is redirected to the money page**:
- **Steps to execute**:
  1. User opens the app
  2. User touches the coin icon
  3. User is redirected to the money page

### 3.6 Test scenario 6
- **End-user**
- **Go to task page**
- **Precondition**:
  1. The user is logged in
- **User is redirected to the task page**:
- **Steps to execute**:
  1. User opens the app
  2. User touches the task icon
  3. User is redirected to the task page

### 3.7 Test scenario 7
- **End-user**
- **Switch language**
- **Precondition**:
  1. The user is logged in
  2. The user is on the settings page
- **Language is switched to the selected language**:
- **Steps to execute**:
  1. User opens the app and navigates to the settings page
  2. User clicks on the language button
  3. User selects another language from the list
  4. The application language is updated to the selected language
  
---

## 4. Coverage of key user journeys
Ensure the test scenarios reflect real-world use cases and cover all major interactions, including edge cases or failure points.

### 4.1 Key user journeys
- **Journey 1**: **New user installing the app**
  - A new user downloads the app, creates an account using their Google account, they join a collocation, they set up their profile. They explore the wall page, add a post, and check the tasks page.
  
- **Journey 2**: **Daily usage**
  - A registered user logs in, checks the wall page for updates, adds a new post, assigns a task to a roommate on the tasks page, and updates their notification preferences in the settings page.

- **Journey 3**: **Language and settings management**
  - A user goes to the settings page, switches the app language, adjusts their notification preferences to customize their experience, after en error with their notifications, they submit a bug report.

### 4.2 Edge cases and failure points
- **Edge Case 1**: **Failed google account connection**
  - Description: The user attempts to connect their Google account, they encounters an error (network issues)
  - Testing: Simulate a failed connection by disabling internet access. Verify that the app displays an appropriate error message and allows the user to retry.

- **Edge Case 2**: **Post deletion by unauthorized user**
  - Description: A user attempts to delete a post while not having propper credentials.
  - Testing: Verify that the posts and their “x” button is only visible to authorized users. Ensure unauthorized users cannot delete or see posts.

- **Edge Case 3**: **Task assignment to non-existing user**
  - Description: A user tries to assign a task to a user who is no longer part of the collocation.
  - Testing: Simulate assigning a task to a deleted user. Verify that the app does not display the deleted user or displays an error message and prevents the assignment (eg. user existed when the app was getting the collocation users).

- **Edge Case 4**: **App crash during post creation**
  - Description: The app crashes while the user is creating a post.
  - Testing: Simulate an app crash during post creation. Verify that the app recovers and does not lose the user's input.

---

## 5. Clear evaluation criteria
Define success metrics and baseline performance or usability expectations to determine whether a feature is ready for release.

### 5.1 Success metrics
- **Metric 1**: (e.g., 95% of testers successfully complete the login process)
- **Metric 2**: (e.g., average response time for API calls is under 500ms)

### 5.2 Baseline expectations
- **Performance Baseline**: (e.g., system handles 100 concurrent users without degradation)
- **Usability Baseline**: (e.g., 90% of testers rate the interface as "easy to use")

---
