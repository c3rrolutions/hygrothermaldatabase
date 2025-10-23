export default {
  home: "/",
  legalNotice: "/legal-notice",
  dataProtectionInformation: "/data-protection-information",
  antiforgeryToken: "/antiforgery/token",
  userInfo: "/user-info",
  database: "/database",
  calorimetricData: "/data/calorimetric",
  calorimetricDatum(uuid: string) {
    return `/data/calorimetric/${encodeURIComponent(uuid)}`;
  },
  hygrothermalData: "/data/hygrothermal",
  hygrothermalDatum(uuid: string) {
    return `/data/hygrothermal/${encodeURIComponent(uuid)}`;
  },
  opticalData: "/data/optical",
  opticalDatum(uuid: string) {
    return `/data/optical/${encodeURIComponent(uuid)}`;
  },
  photovoltaicData: "/data/photovoltaic",
  photovoltaicDatum(uuid: string) {
    return `/data/photovoltaic/${encodeURIComponent(uuid)}`;
  },
  geometricData: "/data/geometric",
  geometricDatum(uuid: string) {
    return `/data/geometric/${encodeURIComponent(uuid)}`;
  },
  createData: "/data/create",
  uploadFile: "/upload-file",
  login: "/connect/login",
  logout: "/connect/logout",
  graphQl: new URL("/graphql/", process.env.NEXT_PUBLIC_DATABASE_URL).href,
  metabase: {
    home: process.env.NEXT_PUBLIC_METABASE_URL,
    components: new URL("/components", process.env.NEXT_PUBLIC_METABASE_URL).href,
    component(uuid: string) {
      return new URL(`/components/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
    databases: new URL("/databases", process.env.NEXT_PUBLIC_METABASE_URL).href,
    database(uuid: string) {
      return new URL(`/databases/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
    dataFormats: new URL(`/data-formats`, process.env.NEXT_PUBLIC_METABASE_URL).href,
    dataFormat(uuid: string) {
      return new URL(`/data-formats/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
    institutions: new URL("/institutions", process.env.NEXT_PUBLIC_METABASE_URL).href,
    institution(uuid: string) {
      return new URL(`/institutions/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
    methods: new URL("/methods", process.env.NEXT_PUBLIC_METABASE_URL).href,
    method(uuid: string) {
      return new URL(`/methods/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
    users: new URL("/users", process.env.NEXT_PUBLIC_METABASE_URL).href,
    user(uuid: string) {
      return new URL(`/users/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
    allCalorimetricData: new URL(`/data/calorimetric`, process.env.NEXT_PUBLIC_METABASE_URL).href,
    allGeometricData: new URL(`/data/geometric`, process.env.NEXT_PUBLIC_METABASE_URL).href,
    allHygrothermalData: new URL(`/data/hygrothermal`, process.env.NEXT_PUBLIC_METABASE_URL).href,
    allOpticalData: new URL(`/data/optical`, process.env.NEXT_PUBLIC_METABASE_URL).href,
    allPhotovoltaicData: new URL(`/data/photovoltaic`, process.env.NEXT_PUBLIC_METABASE_URL).href,
    graphQl: new URL("/graphql/", process.env.NEXT_PUBLIC_METABASE_URL).href,
  },
};
