import { useRouter } from "next/router";
import GeometricData from "../../../components/data/geometric/GeometricData";
import Layout from "../../../components/Layout";

function Page() {
  const router = useRouter();

  if (!router.isReady) {
    return null;
  }

  const { uuid } = router.query;

  return (
    <Layout>
      <GeometricData geometricDataId={uuid} />
    </Layout>
  );
}
export default Page;
