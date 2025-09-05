# Hestia Acceptance Test Scenarios

## 1. User Registration

- **Functionality:** Register new users.
- **Objective:** Ensure new users can create accounts.
- **Prerequisites:** Google account.
- **Steps:**
   1. Open the registration page.
   2. Fill in the required fields (name, *colocationID*).
   3. Submit the registration form.
- **Expected result:** User is registered and logged in.

---

## 2. User Authentication & Colocation Management

- **Functionality:** Secure login, join/create colocation, manage membership, exit group.
- **Objective:** Test authentication and group management.
- **Prerequisites:** Google account; hestia account; application connected to authentication provider.
- **Steps:**
   1. Log in using Google.
   2. Create a new colocation or join an existing one.
   3. Invite another user.
   4. Exit the colocation.
- **Expected result:** Authentication succeeds and users can join/leave as expected.

---

## 3. Page Navigation

- **Functionality:** Page navigation via navbar and menu buttons.
- **Objective:** Ensure users can access all pages and contents.
- **Prerequisites:** Application deployed and running; any user account currently in a colocation group.
- **Steps:**
   1. Log in to the application.
   2. Use the navbar/menu to navigate to each main page (Chores, Budget, Messages, etc.).
   3. Attempt to return to the index (reminder) page.
- **Expected result:** All pages load correctly, navigation is smooth, and no dead ends or inaccessible pages.

---

## 4. Chore Management

- **Functionality:** Assign, track, and remind household tasks.
- **Objective:** Verify users can manage chores.
- **Prerequisites:** At least two users in the same colocation.
- **Steps:**
   1. Log in as User A.
   2. Create a new chore and assign it to User B.
   3. Log in as User B and view assigned chores.
   4. Mark the chore as completed.
- **Expected result:** Chore is visible to both users, assignment and completion status update correctly.

---

## 5. Budget Tracking

- **Functionality:** Manage shared expenses and monitor payments.
- **Objective:** Ensure users can add and track expenses.
- **Prerequisites:** At least two users in the same colocation.
- **Steps:**
   1. Log in as User A.
   2. Add a new shared expense.
   3. Log in as User B and view the expense.
   4. Mark the expense as paid.
- **Expected result:** Expense is visible to all, payment status updates, and totals are recalculated.

---

## 6. Debt calculation

- **Functionality:** Calculate the lowest ammount of reimbursement across all expenses.
- **Objective:** Ensure all debts are paid after all reimbursements.
- **Prerequisites:** At least three users in the same colocation.
- **Steps:**
   1. Log in as User A.
   2. Create a new expense and mark it as paid.
   3. Log in as User B and create a new expense.
   4. Log in as User C and create a new expense.
   5. Calculate the total amount owed by each user.
   6. Make reimbursements between users.
- **Expected result:** All debts are settled, and no user has an outstanding balance.

---

## 7. Communication Tools

- **Functionality:** Send and receive messages between roommates.
- **Objective:** Ensure messaging works in real time.
- **Prerequisites:** At least two users in the same colocation.
- **Steps:**
   1. User A sends a message to the group.
   2. User B receives the message.
   3. User B replies.
- **Expected result:** Messages are delivered instantly and displayed in the conversation thread.

---

## 8. Reminders

- **Functionality:** Set reminders for important information.
- **Objective:** Verify reminders can be created and trigger notifications.
- **Prerequisites:** User logged in; notifications enabled.
- **Steps:**
   1. Create a new reminder.
- **Expected result:** Reminder notification is sent to all other roommates.

---

## 9. Languages

- **Functionality:** Choose application language.
- **Objective:** Ensure all text fields update to the selected language.
- **Prerequisites:** User is logged in.
- **Steps:**
   1. Open profile page.
   2. Open settings page.
   3. Change the language.
   4. Navigate through the app.
- **Expected result:** All interface text updates to the selected language.

---

## 10. Notifications

- **Functionality:** Push notifications for updates, messages, and reminders.
- **Objective:** Confirm notifications are sent and received.
- **Prerequisites:** Notifications enabled on device.
- **Steps:**
   1. Trigger an event (e.g., new message, reminder).
   2. Observe notification on device.
- **Expected result:** Notification appears promptly and contains correct information.

---

## 11. Live Updates

- **Functionality:** Live content updates across devices.
- **Objective:** Ensure changes are reflected in real time.
- **Prerequisites:** Two devices logged in to the same colocation.
- **Steps:**
   1. Make a change on Device A (e.g., add a chore).
   2. Observe Device B.
- **Expected result:** Change appears instantly on Device B.

---

## 12. Reactions

- **Functionality:** React to reminders with emojis/icons.
- **Objective:** Test reaction feature.
- **Prerequisites:** At least two users in the same colocation.
- **Steps:**
   1. User A creates a reminder.
   2. User B reacts to the reminder.
- **Expected result:** Reaction is visible to all users.

---

## 13. Image Sharing

- **Functionality:** Send images to colocation members.
- **Objective:** Verify image upload and display.
- **Prerequisites:** Supported image file; users in the same colocation.
- **Steps:**
   1. User A uploads and sends an image on the reminders page.
   2. User B views the image.
- **Expected result:** Image is displayed correctly to all users.

---

## 14. Groceries Lists

- **Functionality:** Create groceries lists as checklists.
- **Objective:** Ensure users can manage shared grocery lists.
- **Prerequisites:** At least two users in the same colocation.
- **Steps:**
   1. Create a new groceries list on the reminders page.
   2. Add items to the list.
   3. Mark items as bought.
- **Expected result:** List updates are visible to all; items can be checked off.

---

## 15. Multiple Tasks Views

- **Functionality:** Display tasks in list or calendar view.
- **Objective:** Test switching between task views.
- **Prerequisites:** Tasks exist in the system. User is logged in.
- **Steps:**
   1. Open the tasks page.
   2. Switch between list and calendar views.
- **Expected result:** Tasks are displayed correctly in both views.

---

## 16. Display Theme

- **Functionality:** Set color theme (light/dark/system).
- **Objective:** Ensure theme changes are applied.
- **Prerequisites:** User is logged in.
- **Steps:**
   1. Open settings.
   2. Change the display theme.
- **Expected result:** Application updates to the selected theme immediately.

---

## 17. Responsiveness

- **Functionality:** Layout adapts to different screen sizes.
- **Objective:** Test responsiveness and animation display.
- **Prerequisites:** Devices of various sizes (mobile, tablet, desktop).
- **Steps:**
   1. Open the app on different devices.
   2. Navigate through pages and observe animations.
- **Expected result:** Layout and animations display correctly on all devices.

---

## 18. Browser Support

- **Functionality:** Works on Chrome, Firefox, Safari, Edge.
- **Objective:** Ensure cross-browser compatibility.
- **Prerequisites:** Access to all supported browsers.
- **Steps:**
   1. Open the app in each browser.
   2. Navigate through all pages.
- **Expected result:** Layout and functionality are consistent across browsers.

---

## 19. Operating Systems

- **Functionality:** Compatible with Android, iOS, and desktop OS.
- **Objective:** Test OS compatibility.
- **Prerequisites:** Devices with different operating systems.
- **Steps:**
   1. Open the app on each OS.
   2. Perform basic operations.
- **Expected result:** App functions correctly on all supported OS.

---

## 20. Device Types

- **Functionality:** Functional on smartphones, tablets, and desktops.
- **Objective:** Ensure device compatibility.
- **Prerequisites:** Access to all device types.
- **Steps:**
   1. Open the app on each device type.
   2. Use main features.
- **Expected result:** All features work as expected on all device types.