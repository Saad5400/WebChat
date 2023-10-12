import { localStorageStore } from '@skeletonlabs/skeleton';
import type { Writable } from 'svelte/store';

const authStore: Writable<{ accessToken: string; refreshToken: string }> = localStorageStore('auth', {
    accessToken: '',
    refreshToken: '',
});

export default authStore;
