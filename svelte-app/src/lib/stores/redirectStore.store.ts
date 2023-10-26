import { writable, type Writable } from 'svelte/store';

const redirectStore: Writable<string> = writable('');

export default redirectStore;
