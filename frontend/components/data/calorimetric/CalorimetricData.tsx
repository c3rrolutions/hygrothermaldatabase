import { Scalars } from "../../../__generated__/graphql";
import { CalorimetricDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result, Descriptions, message } from "antd";
import { useEffect } from "react";
import { stringifyApolloError } from "../../../lib/apollo";
import DataPageHeader from "../DataPageHeader";
import { useQuery } from "@apollo/client/react";

export type CalorimetricDataProps = {
  calorimetricDataId: Scalars["Uuid"]["input"];
};

export default function CalorimetricData({ calorimetricDataId }: CalorimetricDataProps) {
  const { loading, error, data } = useQuery(CalorimetricDataDocument, {
    variables: {
      uuid: calorimetricDataId,
    },
  });
  const calorimetricData = data?.calorimetricData;
  const [messageApi, contextHolder] = message.useMessage();

  useEffect(() => {
    if (error) {
      messageApi.error(stringifyApolloError(error));
    }
  }, [error]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!calorimetricData) {
    return (
      <Result
        status="500"
        title="500"
        subTitle="Sorry, something went wrong."
      />
    );
  }

  return (
    <>
      {contextHolder}
    <DataPageHeader
      data={calorimetricData}
    // extra={[
    //   <UpdateCalorimetricData
    //     key="updateCalorimetricData"
    //     calorimetricDataId={calorimetricData.uuid}
    //   />,
    // ]}
    >
      <Descriptions.Item key="gValues" label="g Values">
        {calorimetricData.gValues.map((x) => x.toLocaleString("en")).join(", ")}
      </Descriptions.Item>
      <Descriptions.Item key="uValues" label="u Values">
        {calorimetricData.uValues.map((x) => x.toLocaleString("en")).join(", ")}
      </Descriptions.Item>
    </DataPageHeader>
      </>
  );
}
