# Événements SignalR

Ce document liste les codes d'événements utilisés dans la communication SignalR entre le backend et le frontend.

### Reminders
| Code événement | Description | Payload (données envoyées) |
|--|--|--|
| `NewReminderAdded` | Un nouveau rappel a été ajouté | ReminderOutput |
| `ReminderUpdated` | Un rappel existant a été modifié | ReminderOutput |
| `RemindersUpdated` | Des rappels existants ont été modifié | ReminderOutput |
| `ReminderDeleted` | Un rappel a été supprimé | Guid |

### ShoppingList
| Code événement | Description | Payload (données envoyées) |
|--|--|--|
| `NewShoppingItem` | Un objet de liste de course a été ajouté | ShoppingItemOutput |
| `UpdatedShoppingItem` | Un objet de liste de course a été modifié | ShoppingItemOutput |
| `DeletedShoppingItem` | Un objet de liste de course a été supprimé | GUID |

### Reactions
| Code événement | Description | Payload (données envoyées) |
|--|--|--|
| `NewReaction` | Une reaction a été ajoutée | ReactionOutput |
| `UpdateReaction` | Une reaction a été modifiée | ReactionOutput |
| `DeleteReaction` | Une reaction a été supprimée | GUID |

### PollVote
| Code événement | Description | Payload (données envoyées) |
|--|--|--|
| `NewPollVote` | Une reponse de sondage a été ajoutée | PollVoteOutput |
| `UpdatePollVote` | Une reponse de sondage a été modifiée | PollVoteOutput |
| `DeletePollVote` | Une reponse de sondage a été supprimée | GUID |

### Chores
| Code événement | Description | Payload (données envoyées) |
|--|--|--|
| `NewChoreAdded` | Une nouvelle tache a été ajoutée | ChoreOutput |
| `NewChoreMessageAdded` | Un nouveau commentaire de tache a été ajoutée | ChoreMessageOutput |
| `ChoreUpdated` | Une tache existante a été modifiée | ChoreOutput |
| `ChoreDeleted` | Une tache a été supprimée | Guid |
| `ChoreMessagesDeleted` | Un commentaire de tache a été supprimée | Guid |
| `ChoreEnrollmentAdded` | Une assignation a une tache a été ajoutée | {ChoreID, UserId} |
| `ChoreEnrollmentRemoved` | Une assignation a une tache a été supprimée | {ChoreID, UserId} |

### Expenses
| Code événement | Description | Payload (données envoyées) |
|--|--|--|
| `NewExpenseCategoryAdded` | Une categorie de dépenses a été ajoutée | ExpenseCategoryOutput |
| `ExpenseCategoryUpdated` | Une categorie de dépenses a été modifiée | ExpenseCategoryOutput |
| `ExpenseCategoryDeleted` | Une categorie de dépenses a été supprimée | GUID |
| `NewExpenseAdded` | Une dépense a été ajoutée | Guid |
| `ExpenseUpdated` | Une dépense a été modifiée | Guid |
| `ExpenseDeleted` | Une dépense a été supprimée | {ChoreID, UserId} |

### Messsages 
| Code événement | Description | Payload (données envoyées) |
|--|--|--|
| `NewMessageAdded` | Un message a été ajouté | MessageOutput |
| `MessageUpdated` | Un message a été modifié | MessageOutput |
| `MessageDeleted` | Un message a été supprimé | GUID |

---

## Notes

- Tous les noms d’événements sont sensibles à la casse.
- Le frontend doit utiliser exactement ces noms pour s’abonner aux événements.
- Mettre à jour ce document dès qu’un nouvel événement est ajouté ou modifié.
- Le contenu (`payload`) de chaque événement peut être étendu selon les besoins.
