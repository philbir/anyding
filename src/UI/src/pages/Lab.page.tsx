import { Text } from "@mantine/core";
import type React from "react";
import MonacoEditor from "react-monaco-editor";

const LabPage: React.FC = () => {
	const options = {
		selectOnLineNumbers: true,
	};

	return (
		<div>
			<Text size="xl">Lab</Text>
			<MonacoEditor
				width="400"
				height="600"
				language="javascript"
				theme="vs-light"
				value={"Code"}
				options={options}
			/>
		</div>
	);
};

export default LabPage;
