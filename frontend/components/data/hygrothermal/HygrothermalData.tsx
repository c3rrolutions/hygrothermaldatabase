import { Scalars } from "../../../__generated__/graphql";
import { HygrothermalDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result, message } from "antd";
import { useEffect } from "react";
import { stringifyApolloError } from "../../../lib/apollo";
import DataPageHeader from "../DataPageHeader";
import { useQuery } from "@apollo/client/react";

export type HygrothermalDataProps = {
  hygrothermalDataId: Scalars["Uuid"]["input"];
};

export default function HygrothermalData({
  hygrothermalDataId,
}: HygrothermalDataProps) {
  const { loading, error, data } = useQuery(HygrothermalDataDocument, {
    variables: {
      uuid: hygrothermalDataId,
    },
  });
  const hygrothermalData = data?.hygrothermalData;
  const [messageApi, contextHolder] = message.useMessage();

  useEffect(() => {
    if (error) {
      messageApi.error(stringifyApolloError(error));
    }
  }, [error]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!hygrothermalData) {
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
        data={hygrothermalData}
        // extra={[
        //   <UpdateHygrothermalData
        //     key="updateHygrothermalData"
        //     hygrothermalDataId={hygrothermalData.uuid}
        //   />,
        // ]}
      ></DataPageHeader>
    </>
  );
}
