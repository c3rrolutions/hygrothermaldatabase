import { useRouter } from "next/router";
import GeometricData from "../../../components/data/geometric/GeometricData";
import Layout from "../../../components/Layout";
import { Skeleton } from "antd";

function Page() {
  const router = useRouter();
  const { uuid } = router.query;

  if (!uuid) {
    return (
      <Layout>
        <Skeleton active avatar title />
      </Layout>
    );
  }

  return (
    <Layout>
      <GeometricData id={String(uuid)} />
    </Layout>
  );
}

export default Page;
