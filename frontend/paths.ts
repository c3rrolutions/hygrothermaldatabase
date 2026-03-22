import { Route } from "next";

const metabaseUrl = "https://www.buildingenvelopedata.org";

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
  lifeCycleData: "/data/life-cycle" as Route,
  lifeCycleDatum(uuid: string) {
    return `/data/life-cycle/${encodeURIComponent(uuid)}` as Route;
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
  graphQl: "/graphql/" as Route,
  metabase: {
    home: metabaseUrl,
    components: new URL("/components", metabaseUrl).href as Route,
    component(uuid: string) {
      return new URL(`/components/${encodeURIComponent(uuid)}`, metabaseUrl)
        .href as Route;
    },
    databases: new URL("/databases", metabaseUrl).href as Route,
    database(uuid: string) {
      return new URL(`/databases/${encodeURIComponent(uuid)}`, metabaseUrl)
        .href as Route;
    },
    dataFormats: new URL(`/data-formats`, metabaseUrl).href as Route,
    dataFormat(uuid: string) {
      return new URL(`/data-formats/${encodeURIComponent(uuid)}`, metabaseUrl)
        .href as Route;
    },
    institutions: new URL("/institutions", metabaseUrl).href as Route,
    institution(uuid: string) {
      return new URL(`/institutions/${encodeURIComponent(uuid)}`, metabaseUrl)
        .href as Route;
    },
    methods: new URL("/methods", metabaseUrl).href as Route,
    method(uuid: string) {
      return new URL(`/methods/${encodeURIComponent(uuid)}`, metabaseUrl)
        .href as Route;
    },
    users: new URL("/users", metabaseUrl).href as Route,
    user(uuid: string) {
      return new URL(`/users/${encodeURIComponent(uuid)}`, metabaseUrl)
        .href as Route;
    },
    allCalorimetricData: new URL(`/data/calorimetric`, metabaseUrl)
      .href as Route,
    allGeometricData: new URL(`/data/geometric`, metabaseUrl).href as Route,
    allHygrothermalData: new URL(`/data/hygrothermal`, metabaseUrl)
      .href as Route,
    alllifeCycleData: new URL(`/data/life-cycle`, metabaseUrl).href as Route,
    allOpticalData: new URL(`/data/optical`, metabaseUrl).href as Route,
    allPhotovoltaicData: new URL(`/data/photovoltaic`, metabaseUrl)
      .href as Route,
    graphQl: new URL("/graphql/", metabaseUrl).href as Route,
  },
};
