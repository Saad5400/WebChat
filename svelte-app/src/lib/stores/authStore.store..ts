import { localStorageStore } from '@skeletonlabs/skeleton';
import type { Writable } from 'svelte/store';

const authStore: Writable<{ accessToken?: string; refreshToken?: string, email?: string, id?: string } | null> = localStorageStore('auth', null);

export default authStore;
