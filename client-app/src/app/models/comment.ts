import { Activity } from "./activity";
import { User } from "./user";

export interface ChatComment{
    id: number;
    body: string;
    username: string;
    displayName: string;
    image: string;
    createdAt: Date;
}