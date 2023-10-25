import { writable } from "svelte/store"
import type { Writable } from 'svelte/store';

const loadingStore: Writable<boolean> = writable(false);

export default loadingStore;