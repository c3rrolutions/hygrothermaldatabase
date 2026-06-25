import { useRouter } from "next/router";
import CalorimetricData from "../../../components/data/calorimetric/CalorimetricData";
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
      <CalorimetricData id={String(uuid)} />
    </Layout>
  );
}

export default Page;
