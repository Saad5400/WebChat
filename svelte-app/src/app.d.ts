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
	id: number?;
	email: string;
}
interface Message {
	id: string;
	host?: boolean;
	text: string;
	timestamp: string;
}
interface Chat {
	user: User;
	lastMessage: Message;
}
