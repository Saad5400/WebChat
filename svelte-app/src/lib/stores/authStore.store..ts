import { localStorageStore } from '@skeletonlabs/skeleton';
import type { Writable } from 'svelte/store';

const authStore: Writable<{ accessToken: string; refreshToken: string, email: string, id: string }> = localStorageStore('auth', {
    accessToken: '',
    refreshToken: '',
    email: '',
    id: '',
});

export default authStore;
