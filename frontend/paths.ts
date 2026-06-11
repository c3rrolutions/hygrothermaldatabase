import { Route } from "next";
import { DataKind, Scalars } from "./__generated__/graphql";

const metabaseUrl = "https://www.buildingenvelopedata.org";

export default {
  home: "/" as Route,
  legalNotice: "/legal-notice" as Route,
  dataProtectionInformation: "/data-protection-information" as Route,
  antiforgeryToken: "/antiforgery/token" as Route,
  userInfo: "/user-info" as Route,
  database: "/database" as Route,
  allCalorimetricData: "/data/calorimetric" as Route,
  calorimetricData: (id: Scalars["Uuid"]["output"]) =>
    `/data/calorimetric/${encodeURIComponent(String(id))}` as Route,
  allGeometricData: "/data/geometric" as Route,
  geometricData: (id: Scalars["Uuid"]["output"]) =>
    `/data/geometric/${encodeURIComponent(String(id))}` as Route,
  allHygrothermalData: "/data/hygrothermal" as Route,
  hygrothermalData: (id: Scalars["Uuid"]["output"]) =>
    `/data/hygrothermal/${encodeURIComponent(String(id))}` as Route,
  allLifeCycleData: "/data/life-cycle" as Route,
  lifeCycleData: (id: Scalars["Uuid"]["output"]) =>
    `/data/life-cycle/${encodeURIComponent(String(id))}` as Route,
  allOpticalData: "/data/optical" as Route,
  opticalData: (id: Scalars["Uuid"]["output"]) =>
    `/data/optical/${encodeURIComponent(String(id))}` as Route,
  allPhotovoltaicData: "/data/photovoltaic" as Route,
  photovoltaicData: (id: Scalars["Uuid"]["output"]) =>
    `/data/photovoltaic/${encodeURIComponent(String(id))}` as Route,
  createData: "/data/create" as Route,
  uploadFile: "/upload-file" as Route,
  resource: (id: Scalars["Uuid"]["output"]) =>
    `/api/resources/${encodeURIComponent(String(id))}` as Route,
  login: "/connect/login" as Route,
  logout: "/connect/logout" as Route,
  graphQl: "/graphql/" as Route,
  metabase: {
    home: metabaseUrl,
    graphQl: new URL("/graphql/", metabaseUrl).href as Route,
    register: new URL("/users/register", metabaseUrl).href as Route,
    components: new URL("/components", metabaseUrl).href as Route,
    component: (id: Scalars["Uuid"]["output"]) =>
      new URL(`/components/${encodeURIComponent(String(id))}`, metabaseUrl)
        .href as Route,
    databases: new URL("/databases", metabaseUrl).href as Route,
    database: (id: Scalars["Uuid"]["output"]) =>
      new URL(`/databases/${encodeURIComponent(String(id))}`, metabaseUrl)
        .href as Route,
    dataFormats: new URL(`/data-formats`, metabaseUrl).href as Route,
    dataFormat: (id: Scalars["Uuid"]["output"]) =>
      new URL(`/data-formats/${encodeURIComponent(String(id))}`, metabaseUrl)
        .href as Route,
    gnuPgKeys: new URL("/gnupg-keys", metabaseUrl).href as Route,
    gnuPgKey: (fingerprint: string) =>
      new URL(`/gnupg-keys/${encodeURIComponent(fingerprint)}`, metabaseUrl)
        .href as Route,
    institutions: new URL("/institutions", metabaseUrl).href as Route,
    institution: (id: Scalars["Uuid"]["output"]) =>
      new URL(`/institutions/${encodeURIComponent(String(id))}`, metabaseUrl)
        .href as Route,
    methods: new URL("/methods", metabaseUrl).href as Route,
    method: (id: Scalars["Uuid"]["output"]) =>
      new URL(`/methods/${encodeURIComponent(String(id))}`, metabaseUrl)
        .href as Route,
    users: new URL("/users", metabaseUrl).href as Route,
    user: (id: Scalars["Uuid"]["output"]) =>
      new URL(`/users/${encodeURIComponent(String(id))}`, metabaseUrl)
        .href as Route,
    allData: "/data" as Route,
    data(
      databaseId: Scalars["Uuid"]["output"],
      dataKind: DataKind,
      id: Scalars["Uuid"]["output"],
    ) {
      switch (dataKind) {
        case DataKind.CalorimetricData:
          return this.calorimetricData(databaseId, id);
        case DataKind.GeometricData:
          return this.geometricData(databaseId, id);
        case DataKind.HygrothermalData:
          return this.hygrothermalData(databaseId, id);
        case DataKind.LifeCycleData:
          return this.lifeCycleData(databaseId, id);
        case DataKind.OpticalData:
          return this.opticalData(databaseId, id);
        case DataKind.PhotovoltaicData:
          return this.photovoltaicData(databaseId, id);
        default:
          return assertNever(dataKind);
      }
    },
    allCalorimetricData: new URL(`/data/calorimetric`, metabaseUrl)
      .href as Route,
    calorimetricData: (
      databaseId: Scalars["Uuid"]["output"],
      id: Scalars["Uuid"]["output"],
    ) =>
      `/databases/${encodeURIComponent(databaseId)}/data/calorimetric/${encodeURIComponent(id)}` as Route,
    allGeometricData: new URL(`/data/geometric`, metabaseUrl).href as Route,
    geometricData: (
      databaseId: Scalars["Uuid"]["output"],
      id: Scalars["Uuid"]["output"],
    ) =>
      `/databases/${encodeURIComponent(databaseId)}/data/geometric/${encodeURIComponent(id)}` as Route,
    allHygrothermalData: new URL(`/data/hygrothermal`, metabaseUrl)
      .href as Route,
    hygrothermalData: (
      databaseId: Scalars["Uuid"]["output"],
      id: Scalars["Uuid"]["output"],
    ) =>
      `/databases/${encodeURIComponent(databaseId)}/data/hygrothermal/${encodeURIComponent(id)}` as Route,
    allLifeCycleData: new URL(`/data/life-cycle`, metabaseUrl).href as Route,
    lifeCycleData: (
      databaseId: Scalars["Uuid"]["output"],
      id: Scalars["Uuid"]["output"],
    ) =>
      `/databases/${encodeURIComponent(databaseId)}/data/LifeCycle/${encodeURIComponent(id)}` as Route,
    allOpticalData: new URL(`/data/optical`, metabaseUrl).href as Route,
    opticalData: (
      databaseId: Scalars["Uuid"]["output"],
      id: Scalars["Uuid"]["output"],
    ) =>
      `/databases/${encodeURIComponent(databaseId)}/data/optical/${encodeURIComponent(id)}` as Route,
    allPhotovoltaicData: new URL(`/data/photovoltaic`, metabaseUrl)
      .href as Route,
    photovoltaicData: (
      databaseId: Scalars["Uuid"]["output"],
      id: Scalars["Uuid"]["output"],
    ) =>
      `/databases/${encodeURIComponent(databaseId)}/data/photovoltaic/${encodeURIComponent(id)}` as Route,
  },
};
