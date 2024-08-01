// https://nuxt.com/docs/api/configuration/nuxt-config

import { defineNuxtConfig } from 'nuxt/config'
import { spawn } from "child_process";
import fs from "fs";
import path from "path";
import * as process from "process";

const certificateName = process.env.npm_package_name;

// Get base folder for certificates.
const baseFolder =
    process.env.APPDATA !== undefined && process.env.APPDATA !== ''
        ? `${process.env.APPDATA}/ASP.NET/https`
        : `${process.env.HOME}/.aspnet/https`;

// Define certificate filepath
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
// Define key filepath
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

export default defineNuxtConfig(async ()=> {
  if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    // Wait for the certificate to be generated
    await new Promise<void>((resolve) => {
      spawn('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
      ], { stdio: 'inherit', })
          .on('exit', (code: any) => {
            resolve();
            if (code) {
              process.exit(code);
            }
          });
    });
  }
  
  const config = {
    compatibilityDate: '2024-04-03',
    sourceMap: true,
    devtools: {enabled: true},
    devServer:{
      https:{
        cert: certFilePath,
        key: keyFilePath
      }
    },
    modules: ['@nuxt/eslint']
  }
  return config;
})