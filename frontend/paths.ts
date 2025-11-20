import { Route } from "next";

export default {
  home: "/" as Route,
  legalNotice: "/legal-notice" as Route,
  dataProtectionInformation: "/data-protection-information" as Route,
  antiforgeryToken: "/antiforgery/token" as Route,
  userInfo: "/user-info" as Route,
  database: "/database" as Route,
  calorimetricData: "/data/calorimetric" as Route,
  calorimetricDatum(uuid: string) {
    return `/data/calorimetric/${encodeURIComponent(uuid)}` as Route;
  },
  hygrothermalData: "/data/hygrothermal" as Route,
  hygrothermalDatum(uuid: string) {
    return `/data/hygrothermal/${encodeURIComponent(uuid)}` as Route;
  },
  opticalData: "/data/optical" as Route,
  opticalDatum(uuid: string) {
    return `/data/optical/${encodeURIComponent(uuid)}` as Route;
  },
  photovoltaicData: "/data/photovoltaic" as Route,
  photovoltaicDatum(uuid: string) {
    return `/data/photovoltaic/${encodeURIComponent(uuid)}` as Route;
  },
  geometricData: "/data/geometric" as Route,
  geometricDatum(uuid: string) {
    return `/data/geometric/${encodeURIComponent(uuid)}` as Route;
  },
  createData: "/data/create" as Route,
  uploadFile: "/upload-file" as Route,
  login: "/connect/login" as Route,
  logout: "/connect/logout" as Route,
  graphQl: new URL("/graphql/", process.env.NEXT_PUBLIC_DATABASE_URL)
    .href as Route,
  metabase: {
    home: process.env.NEXT_PUBLIC_METABASE_URL,
    components: new URL("/components", process.env.NEXT_PUBLIC_METABASE_URL)
      .href as Route,
    component(uuid: string) {
      return new URL(
        `/components/${encodeURIComponent(uuid)}`,
        process.env.NEXT_PUBLIC_METABASE_URL,
      ).href as Route;
    },
    databases: new URL("/databases", process.env.NEXT_PUBLIC_METABASE_URL)
      .href as Route,
    database(uuid: string) {
      return new URL(
        `/databases/${encodeURIComponent(uuid)}`,
        process.env.NEXT_PUBLIC_METABASE_URL,
      ).href as Route;
    },
    dataFormats: new URL(`/data-formats`, process.env.NEXT_PUBLIC_METABASE_URL)
      .href as Route,
    dataFormat(uuid: string) {
      return new URL(
        `/data-formats/${encodeURIComponent(uuid)}`,
        process.env.NEXT_PUBLIC_METABASE_URL,
      ).href as Route;
    },
    institutions: new URL("/institutions", process.env.NEXT_PUBLIC_METABASE_URL)
      .href as Route,
    institution(uuid: string) {
      return new URL(
        `/institutions/${encodeURIComponent(uuid)}`,
        process.env.NEXT_PUBLIC_METABASE_URL,
      ).href as Route;
    },
    methods: new URL("/methods", process.env.NEXT_PUBLIC_METABASE_URL)
      .href as Route,
    method(uuid: string) {
      return new URL(
        `/methods/${encodeURIComponent(uuid)}`,
        process.env.NEXT_PUBLIC_METABASE_URL,
      ).href as Route;
    },
    users: new URL("/users", process.env.NEXT_PUBLIC_METABASE_URL)
      .href as Route,
    user(uuid: string) {
      return new URL(
        `/users/${encodeURIComponent(uuid)}`,
        process.env.NEXT_PUBLIC_METABASE_URL,
      ).href as Route;
    },
    allCalorimetricData: new URL(
      `/data/calorimetric`,
      process.env.NEXT_PUBLIC_METABASE_URL,
    ).href as Route,
    allGeometricData: new URL(
      `/data/geometric`,
      process.env.NEXT_PUBLIC_METABASE_URL,
    ).href as Route,
    allHygrothermalData: new URL(
      `/data/hygrothermal`,
      process.env.NEXT_PUBLIC_METABASE_URL,
    ).href as Route,
    allOpticalData: new URL(
      `/data/optical`,
      process.env.NEXT_PUBLIC_METABASE_URL,
    ).href as Route,
    allPhotovoltaicData: new URL(
      `/data/photovoltaic`,
      process.env.NEXT_PUBLIC_METABASE_URL,
    ).href as Route,
    graphQl: new URL("/graphql/", process.env.NEXT_PUBLIC_METABASE_URL)
      .href as Route,
  },
};
