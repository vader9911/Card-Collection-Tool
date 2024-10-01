import { Card } from "./card";


export interface Collection {
  collectionID: number;
  collectionName: string;
  imageUri: string;
  notes: string;
  createdDate: string;
  totalCards: number;   // Add total cards
  totalValue: number;   // Add total value
  cards: Card[];        // Array of cards in the collection
}

