import { loadEnv } from 'vite';

const env = loadEnv('test', process.cwd(), '');

Object.assign(process.env, env);
