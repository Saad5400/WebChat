// useDebounce.ts

export default function useDebounce(delay: number, callback: Function) {
    let timeout: NodeJS.Timeout;

    return (...args: any[]) => {
        clearTimeout(timeout);
        timeout = setTimeout(() => callback(...args), delay);
    };
}