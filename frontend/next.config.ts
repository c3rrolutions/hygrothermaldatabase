import type { NextConfig } from 'next'

const nextConfig: NextConfig = {
  reactStrictMode: true,
  typedRoutes: true,
  allowedDevOrigins:
    process.env.NEXT_PUBLIC_DATABASE_URL == null
      ? []
      : [new URL(process.env.NEXT_PUBLIC_DATABASE_URL).hostname],
  turbopack: {
    rules: {
      '/\.(yml|yaml$)/': [
        'yaml-loader',
      ],
    },
  },
  experimental: {
    typedEnv: true,
  },
  // experimental: {
  //   swcPlugins: [
  //     [
  //       '@swc-contrib/plugin-graphql-codegen-client-preset',
  //       {
  //         artifactDirectory: './__generated__/',
  //         gqlTagName: 'graphql'
  //       }
  //     ]
  //   ]
  // },
}

export default nextConfig
