// See https://kit.svelte.dev/docs/types#app
// for information about these interfaces
// and what to do when importing types
declare namespace App {
	// interface Locals {}
	// interface PageData {}
	// interface Error {}
	// interface Platform {}
}

declare module 'validator/es/lib/*';

interface User {
	id: string;
	email: string;
}
interface Message {
	text: string;
	createdAt?: string;
	isRead?: boolean;
	receiverId?: string;
	senderId?: string;
	sender?: User;
	receiver?: User;
}
interface Chat {
	user: User;
	lastMessage: Message;
}
